using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Model;

public class GameHub : Hub
{
    private readonly CaroDbContext _context;

    public GameHub(CaroDbContext context)
    {
        _context = context;
    }

    public async Task MakeMove(Guid matchId, string playerId, int x, int y)
    {
        // Kiểm tra hợp lệ
        if (!IsValidMove(matchId, x, y))
        {
            await Clients.Caller.SendAsync("InvalidMove", "Nước đi không hợp lệ.");
            return;
        }

        var match = await _context.GameMatches.FindAsync(matchId);
        if (match == null) return;

        // Kiểm tra lượt chơi
        if (match.NextTurnPlayerID != playerId)
        {
            await Clients.Caller.SendAsync("InvalidTurn", "Chưa đến lượt của bạn.");
            return;
        }

        // Lưu nước đi
        var move = new Moves
        {
            ID = Guid.NewGuid(),
            MatchID = matchId,
            PlayerID = playerId,
            X = x,
            Y = y,
            CreateAt = DateTime.Now
        };

        _context.Moves.Add(move);

        // Đổi lượt
        match.NextTurnPlayerID = (playerId == match.Player1ID) ? match.Player2ID : match.Player1ID;

        _context.GameMatches.Update(match);
        await _context.SaveChangesAsync();

        // Gửi nước đi đến cả phòng
        string roomId = match.RoomId.ToString();
        await Clients.Group(roomId).SendAsync("MoveMade", playerId, x, y, match.NextTurnPlayerID);

        // Kiểm tra thắng
        if (CheckGameOver(match))
        {
            string winnerId = match.WinnerID;

            // 🔥 Truy vấn tên người thắng
            var winner = await _context.Users
                .FirstOrDefaultAsync(u => u.Id.ToString() == winnerId);

            string fullName = $"{winner.Firstname} {winner.Lastname}";

            // Gửi cả ID và tên về client
            await Clients.Group(roomId).SendAsync("GameOver", new
            {
                userId = winnerId,
                fullName = fullName
            });
        }
    }

    private bool IsValidMove(Guid matchId, int x, int y)
    {
        return !_context.Moves.Any(m => m.MatchID == matchId && m.X == x && m.Y == y);
    }

    private bool CheckGameOver(GameMatches match)
    {
        var moves = _context.Moves
            .Where(m => m.MatchID == match.ID)
            .OrderBy(m => m.CreateAt)
            .ToList();

        var board = new string[15, 15];
        string player1 = match.Player1ID;
        string player2 = match.Player2ID;

        foreach (var move in moves)
        {
            string symbol = move.PlayerID == player1 ? "X" : "O";
            board[move.X, move.Y] = symbol;
        }

        for (int x = 0; x < 15; x++)
        {
            for (int y = 0; y < 15; y++)
            {
                string current = board[x, y];
                if (string.IsNullOrEmpty(current)) continue;

                if (CheckDirection(board, x, y, 1, 0, current) ||
                    CheckDirection(board, x, y, 0, 1, current) ||
                    CheckDirection(board, x, y, 1, 1, current) ||
                    CheckDirection(board, x, y, 1, -1, current))
                {
                    // Ghi nhận người thắng
                    match.WinnerID = current == "X" ? player1 : player2;

                    // Cập nhật điểm Elo
                    var winner = _context.Users.FirstOrDefault(u => u.Id == match.WinnerID);
                    var loser = _context.Users.FirstOrDefault(u => u.Id != match.WinnerID && (u.Id == player1 || u.Id == player2));

                    if (winner != null) winner.EloRating += 10;
                    if (loser != null) loser.EloRating = Math.Max(100, loser.EloRating - 10);

                    _context.SaveChanges();
                    return true;
                }
            }
        }

        return false;
    }

    private bool CheckDirection(string[,] board, int x, int y, int dx, int dy, string symbol)
    {
        int count = 1;
        for (int i = 1; i < 5; i++)
        {
            int nx = x + dx * i, ny = y + dy * i;
            if (nx < 0 || ny < 0 || nx >= 15 || ny >= 15) break;
            if (board[nx, ny] != symbol) break;
            count++;
        }
        return count >= 5;
    }
    public async Task ResetGame(Guid matchId)
    {
        var match = await _context.GameMatches
            .Include(m => m.Moves) // nếu có liên kết moves
            .FirstOrDefaultAsync(m => m.ID == matchId);

        if (match != null)
        {
            match.WinnerID = null;
          
            _context.Moves.RemoveRange(match.Moves); // Xoá nước đi cũ
            await _context.SaveChangesAsync();
        }

        // Gửi signal để các client reset giao diện
        await Clients.Group(match.RoomId.ToString()).SendAsync("ResetGame");
    }

    public async Task JoinRoom(string roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        Console.WriteLine($"{Context.ConnectionId} joined room {roomId}");
    }

    public override Task OnConnectedAsync()
    {
        Console.WriteLine($"Connected: {Context.ConnectionId}");
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception ex)
    {
        Console.WriteLine($"Disconnected: {Context.ConnectionId}");
        return base.OnDisconnectedAsync(ex);
    }
}
