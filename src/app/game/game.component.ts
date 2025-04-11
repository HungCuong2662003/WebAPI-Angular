import { Component, OnInit, OnDestroy } from '@angular/core';
import { SharedService } from '../shared.service';
import { Router, NavigationStart ,  Event as NavigationEvent  } from '@angular/router';
import { Subscription } from 'rxjs';
@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.scss']
})
export class GameComponent implements OnInit, OnDestroy {
  board: string[][] = [];
  currentPlayer: string = 'Player 1';
  gameOver: boolean = false;
  winner: string = '';
  profile: any;
  nextTurnPlayerId: string = '';
  private routerSub!: Subscription;
  playersInRoom: { userId: string, fullName: string }[] = [];
  constructor(private SharedService: SharedService, private router: Router) {}

  ngOnInit(): void {
    this.initializeBoard();
    this.loadProfile();
    this.setupSignalR();
    this.listenRouterEvents();
  }

  ngOnDestroy(): void {
    if (this.routerSub) {
      this.routerSub.unsubscribe();
    }
  }

  private setupSignalR(): void {
    this.SharedService.startConnection(); // đã có kiểm tra isConnected ở trong rồi
  
    this.SharedService.onMoveMade().subscribe(({ playerId, x, y }) => {
      this.makeMoveOnBoard(x, y, playerId);
    });
  
    this.SharedService.onGameOver().subscribe((winnerName) => {
      this.winner = winnerName;
      this.gameOver = true;
    });
  
    this.SharedService.userJoined$.subscribe((data) => {
      console.log("Nhận được UserJoined:", data);
      // this.loadPlayersInRoom(); // nếu cần cập nhật
    });
  
    this.SharedService.onResetGame().subscribe(() => {
      console.log("Nhận ResetGame từ server → reset local");
      this.initializeBoard();
      this.gameOver = false;
      this.winner = '';
      this.currentPlayer = 'Player 1';
    });
  
    this.SharedService.listenMatchStarted((matchId: string) => {
      console.log("Nhận MatchStarted:", matchId);
      localStorage.setItem('matchId', matchId);
  
      const userId = this.profile?.id;
      const roomId = localStorage.getItem(`room_${userId}`);
      if (roomId) {
        this.SharedService.joinSignalRRoom(roomId);
      }
    });
  }
  private listenRouterEvents(): void {
    this.routerSub = this.router.events.subscribe((event: NavigationEvent) => {
      if (event instanceof NavigationStart) {
        const currentUrl = this.router.url;
        const nextUrl = event.url;
        if (currentUrl === '/game' && nextUrl !== '/game') {
          this.leaveRoom(false);
        }
      }
    });
  }
  
  private initializeBoard(): void {
    for (let i = 0; i < 10; i++) {
      this.board[i] = [];
      for (let j = 0; j < 10; j++) {
        this.board[i][j] = '';
      }
    }
  }
  public makeMove(x: number, y: number): void {
    const userId = this.profile?.id;
    const matchId = localStorage.getItem('matchId');
  
    if (!userId || !matchId || !this.isValidGuid(matchId)) {
      alert('Thông tin người chơi hoặc mã trận không hợp lệ');
      return;
    }
  
    // ⚠️ KIỂM TRA LƯỢT CHƠI
    const currentTurnPlayerId = this.SharedService.getNextTurnPlayerId();
    if (currentTurnPlayerId !== userId) {
      alert("⛔ Chưa đến lượt của bạn!");
      return;
    }
  
    if (this.board[x][y] === '' && !this.gameOver) {
      this.board[x][y] = this.currentPlayer === 'Player 1' ? 'X' : 'O';
      this.SharedService.makeMove(matchId, userId, x, y);
  
      // Tạm ẩn toggle này, vì lượt chơi thật đã được server xử lý và gửi lại qua MoveMade
      // this.currentPlayer = this.currentPlayer === 'Player 1' ? 'Player 2' : 'Player 1';
    }
  }
  

  private isValidGuid(str: string): boolean {
    const guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
    return guidRegex.test(str);
  }

  private makeMoveOnBoard(x: number, y: number, playerId: string): void {
    const isSelf = playerId === this.profile?.id;
    const symbol = isSelf ? 'X' : 'O';
    this.board[x][y] = symbol;
    this.currentPlayer = this.currentPlayer === 'Player 1' ? 'Player 2' : 'Player 1';
  }
  // loadPlayersInRoom(): void {
  //   const userId = this.profile?.id;
  //   const roomId = localStorage.getItem(`room_${userId}`);
  //   if (roomId) {
  //     this.SharedService.getPlayersInRoom(roomId).subscribe({
  //       next: (res) => {
  //         this.playersInRoom = res;
  //       },
  //       error: (err) => {
  //         console.error('Lỗi khi lấy danh sách người chơi:', err);
  //       }
  //     });
  //   }
  // }
  
  resetGame(): void {
    this.initializeBoard();
    this.gameOver = false;
    this.winner = '';
    this.currentPlayer = 'Player 1';
  
    const matchId = localStorage.getItem("matchId");
    if (matchId) {
      this.SharedService.resetGame(matchId); // gửi yêu cầu lên server
    }
  }
  

  loadProfile(): void {
    this.SharedService.getProfile().subscribe({
      next: (res) => {
         this.profile = res;
      //         // 🟢 Gọi sau khi profile đã sẵn sàng
      // this.loadPlayersInRoom();
      },
      error: (err) => {
        console.error('Lỗi khi tải hồ sơ', err);
      }
    });
  }
  startGame() {
    const userId = this.profile?.id;
    const roomId = localStorage.getItem(`room_${userId}`);
  
    if (!roomId) {
      alert('Không tìm thấy phòng hiện tại');
      return;
    }
  
  
    this.SharedService.joinSignalRRoom(roomId).then(() => {
      const requestBody = { roomId: roomId };
      this.SharedService.startMatch(requestBody).subscribe({
        next: (res) => {
          const matchId = res.matchId;
          localStorage.setItem("matchId", matchId);
          console.log("Trận đấu đã bắt đầu:", res);
          alert("Trận đấu đã bắt đầu!");
        },
        error: (err) => {
          console.error("Lỗi khi bắt đầu trận đấu:", err);
          alert(err.error || "Không thể bắt đầu trận đấu.");
        }
      });
    }).catch((err) => {
      console.error("Không thể vào SignalR room trước khi bắt đầu trận:", err);
    });
  }
  
  isLeavingRoom = false;

  async leaveRoom(manual: boolean = false):  Promise<void>  {
    const userId = this.profile?.id;
    if (this.isLeavingRoom) return; // tránh gọi nhiều lần
    this.isLeavingRoom = true;
    const roomId = localStorage.getItem(`room_${userId}`);
    console.log(roomId , userId)
    if (!roomId) {
      alert('Không tìm thấy phòng hiện tại');
      return;
    }

    const requestBody = { roomId: roomId, userId: userId };
    await this.SharedService.OutSignalRRoom(roomId, userId);
    this.SharedService.leaveRoom(requestBody).subscribe({
      next: async (res) => {
        alert('Rời phòng thành công:');
        console.log('Rời phòng thành công:', res);
      
       
        localStorage.removeItem(`room_${userId}`);
       
        if(manual==true){
          this.router.navigate(['/room']);
        }
       
      },
      error: (err) => {
        console.error('Lỗi khi rời phòng:', err);
        alert('Có lỗi xảy ra khi rời phòng');
      }
    });
  }
}