import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';

export interface Room {
  roomCode: string;
  ownerID: string;
  passwordRoom?: string;
  isPublic: boolean;
  status: string;
}

@Injectable({
  providedIn: 'root'
})
export class SharedService {
  readonly APIUrl = "https://localhost:7224/api";
  private hubConnection: HubConnection | undefined;

  private moveMadeSource = new Subject<{ playerId: string, x: number, y: number }>();
  private gameOverSource = new Subject<string>();

  moveMade$ = this.moveMadeSource.asObservable();
  gameOver$ = this.gameOverSource.asObservable();

  private currentUserId: string | null = null;
  private opponentId: string | null = null;
  private nextTurnPlayerId: string = '';

  constructor(private http: HttpClient) {}

  // Đăng ký, đăng nhập
  signUp(user: any): Observable<any> {
    return this.http.post<any>(`${this.APIUrl}/Auth/SignUp`, user);
  }
  signIn(user: any): Observable<any> {
    return this.http.post(`${this.APIUrl}/Auth/SignIn`, user);
  }

  // Lấy hồ sơ người dùng
  getProfile(): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({ 'Authorization': `Bearer ${token}` });
    return this.http.get<any>(`${this.APIUrl}/Auth/Profile`, { headers });
  }

  // Set & Get user info
  setUserInfo(userId: string) {
    this.currentUserId = userId;
  }
  getUserId(): string | null {
    return this.currentUserId;
  }

  setOpponentId(opponentId: string) {
    this.opponentId = opponentId;
  }
  getOpponentId(): string | null {
    return this.opponentId;
  }

  getNextTurnPlayerId(): string {
    return this.nextTurnPlayerId;
  }

  logout(): void {
    localStorage.removeItem('token');
  }

  // API liên quan đến User và Room
  layDSUser(): Observable<any[]> {
    return this.http.get<any[]>(`${this.APIUrl}/User/GetUserortByEloRating`);
  }
  ThemUser(userData: any): Observable<any> {
    return this.http.post<any>(`${this.APIUrl}/Auth/SignUp`, userData);
  }
  layDSroom(): Observable<any[]> {
    return this.http.get<any[]>(`${this.APIUrl}/rooms`);
  }
  getRoomInfo(roomCode: string): Observable<any> {
    return this.http.get<any>(`${this.APIUrl}/rooms/${roomCode}`);
  }
  addRoom(room: Room): Observable<any> {
    return this.http.post<any>(`${this.APIUrl}/Rooms`, room);
  }
  joinRoom(RoomPlayers: any): Observable<any> {
    return this.http.post<any>(`${this.APIUrl}/RoomPlayers/join`, RoomPlayers);
  }
  joinRoomWithPassword(data: any, password: string): Observable<any> {
    return this.http.post<any>(`${this.APIUrl}/RoomPlayers/joinpass?pass=${password}`, data);
  }
  leaveRoom(RoomPlayers: any): Observable<any> {
    return this.http.post<any>(`${this.APIUrl}/RoomPlayers/out`, RoomPlayers);
  }

  // Bắt đầu trận
  startMatch(RoomPlayers: any): Observable<any> {
    return this.http.post<any>(`${this.APIUrl}/GameMatches/StartMatch`, RoomPlayers);
  }

  // Khởi tạo kết nối SignalR
  public startConnection(): void {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl('https://localhost:7224/gameHub', {
        withCredentials: true
      })
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log("✅ SignalR connected successfully");

        // MoveMade từ server
        this.hubConnection!.on("MoveMade", (playerId: string, x: number, y: number, nextTurnPlayerId: string) => {
          console.log("📥 Nhận MoveMade:", playerId, x, y, nextTurnPlayerId);
          this.nextTurnPlayerId = nextTurnPlayerId;
          this.moveMadeSource.next({ playerId, x, y });
        });

        this.hubConnection!.on("GameOver", (winner: { userId: string, fullName: string }) => {
          console.log("🏁 Game kết thúc:", winner.fullName); // Xem log này có in ra không
          this.gameOverSource.next(winner.fullName); // phải gửi fullName vào Observable
        });
        
        this.hubConnection!.on("ResetGame", () => {
          console.log("🔁 Nhận ResetGame từ server");
          this.resetGameSource.next();
        });

      })
      .catch(err => console.error('Lỗi khi kết nối SignalR:', err));
  }
  public listenMatchStarted(callback: (matchId: string) => void): void {
    if (this.hubConnection) {
      this.hubConnection.on("MatchStarted", (matchInfo: any) => {
        console.log("📥 Nhận MatchStarted:", matchInfo);
        
        const { matchId, player1, player2, nextTurn } = matchInfo;
  
        localStorage.setItem("matchId", matchId);
        this.nextTurnPlayerId = nextTurn;
  
        const currentUserId = this.getUserId();
        if (currentUserId) {
          const opponentId = currentUserId === player1 ? player2 : player1;
          this.setOpponentId(opponentId);
        }
  
        callback(matchId); // Gọi callback để component xử lý tiếp
      });
    }
  }
  public resetGame(matchId: string): void {
    if (this.hubConnection) {
      this.hubConnection.invoke("ResetGame", matchId)
        .catch(err => console.error("❌ Lỗi khi gửi ResetGame:", err));
    }
  }
  private resetGameSource = new Subject<void>();

onResetGame(): Observable<void> {
  return this.resetGameSource.asObservable();
}
  
  // Tham gia nhóm SignalR
  public async joinSignalRRoom(roomId: string): Promise<void> {
    if (this.hubConnection) {
      try {
        await this.hubConnection.invoke("JoinRoom", roomId);
        console.log(`✅ Đã vào SignalR room: ${roomId}`);
      } catch (error) {
        console.error("❌ Lỗi khi vào SignalR group:", error);
      }
    } else {
      console.warn("⚠️ Hub chưa được khởi tạo");
    }
  }

  // Gửi nước đi lên server
  public makeMove(matchId: string, playerId: string, x: number, y: number): void {
    if (this.hubConnection) {
      this.hubConnection.invoke('MakeMove', matchId, playerId, x, y)
        .catch(err => console.error('❌ Lỗi khi gửi nước đi:', err));
    }
  }

  public onMoveMade(): Observable<{ playerId: string, x: number, y: number }> {
    return this.moveMade$;
  }

  public onGameOver(): Observable<string> {
    return this.gameOver$;
  }

  // (nếu có API kiểm tra thắng riêng)
  public checkGameOver(matchId: string): Observable<{ isGameOver: boolean, winnerId: string }> {
    return this.http.get<{ isGameOver: boolean, winnerId: string }>(`${this.APIUrl}/GameMatches/CheckGameOver/${matchId}`);
  }
}
