using Microsoft.AspNetCore.SignalR;
using WebAPI.Data;

public class GameHub : Hub
{
    private readonly CaroDbContext _context;

    public GameHub(CaroDbContext context)
    {
        _context = context;
    }

    public async Task MakeMove(Guid matchId, string playerId, int x, int y)
    {
        // Kiểm tra nước đi
        if (!IsValidMove(matchId.ToString(), x, y))
        {
            await Clients.Caller.SendAsync("InvalidMove", "Nước đi không hợp lệ.");
            return;
        }
        //var match = await _context.GameMatches.FindAsync(matchId);
        //if (match == null) return;
        //// Kiểm tra người chơi có phải người đến lượt không
        //if (match.NextTurnPlayerID != playerId)
        //{
        //    await Clients.Caller.SendAsync("InvalidTurn", "Chưa đến lượt của bạn.");
        //    return;
        //}
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
        await _context.SaveChangesAsync();

        // Gửi nước đi cho cả phòng
        var match = await _context.GameMatches.FindAsync(matchId);
        if (match == null) return;

        var roomId = match.RoomId.ToString();

        await Clients.Group(roomId).SendAsync("MoveMade", playerId, x, y);

        if (CheckGameOver(matchId))
        {
            var winner = DetermineWinner(matchId);
            await Clients.Group(roomId).SendAsync("GameOver", winner);
        }
    }

    private bool IsValidMove(string matchId, int x, int y)
    {
        return !_context.Moves.Any(m => m.MatchID.ToString() == matchId && m.X == x && m.Y == y);
    }

    private bool CheckGameOver(Guid matchId)
    {
        var moves = _context.Moves
            .Where(m => m.MatchID == matchId)
            .OrderBy(m => m.CreateAt)
            .ToList();

        var board = new string[15, 15];

        var match = _context.GameMatches.FirstOrDefault(m => m.ID == matchId);
        if (match == null) return false;

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

                if (CheckDirection(board, x, y, 1, 0, current) ||  // →
                    CheckDirection(board, x, y, 0, 1, current) ||  // ↓
                    CheckDirection(board, x, y, 1, 1, current) ||  // ↘
                    CheckDirection(board, x, y, 1, -1, current))   // ↗
                {
                    string winnerId = current == "X" ? player1 : player2;
                    string loserId = current == "X" ? player2 : player1;

                    // ✅ Lưu người thắng
                    match.WinnerID = winnerId;

                    // ✅ Cập nhật Elo
                    var winner = _context.Users.FirstOrDefault(u => u.Id == winnerId);
                    var loser = _context.Users.FirstOrDefault(u => u.Id == loserId);

                    if (winner != null) winner.EloRating += 10;
                    if (loser != null) loser.EloRating = Math.Max(100, loser.EloRating - 10); // Không dưới 100

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

        for (int step = 1; step < 5; step++)
        {
            int nx = x + dx * step;
            int ny = y + dy * step;

            if (nx < 0 || ny < 0 || nx >= 15 || ny >= 15) break;
            if (board[nx, ny] != symbol) break;

            count++;
        }

        return count >= 5;
    }


    private string DetermineWinner(Guid matchId) => "Player1";

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

    public async Task JoinRoom(string roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        Console.WriteLine($"{Context.ConnectionId} joined room {roomId}");
    }
}
