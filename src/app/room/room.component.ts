import { Component, OnInit } from '@angular/core';
import { SharedService } from '../shared.service';
import { Router } from '@angular/router'; // Đảm bảo sử dụng Router ở đây

@Component({
  selector: 'app-room',
  templateUrl: './room.component.html',
  styleUrls: ['./room.component.scss']
})
export class RoomComponent implements OnInit {

  DSRoom: any[] = [];
  selectedRoom: any = null;  // Phòng được chọn khi click
  profile: any;
  showMenu: boolean = false;
  menuPosition: { top: string, left: string } = { top: '0px', left: '0px' };

  constructor(private SharedService: SharedService, private router: Router) {}
  layDSroom(): void {
    this.SharedService.layDSroom().subscribe({
      next: (data) => {
       
        this.DSRoom = (data as any)?.$values || [];

      },
      error: (err) => {
        console.error('Error loading data:', err);
      }
    });
  }
  

  CreateRoom() {
    this.router.navigate(['/createroom']); // Điều hướng tới trang tạo phòng
  }
   // Điều hướng đến trang sửa phòng
   editRoom(id: string): void {
    this.router.navigate(['/roomedit', id]);
  }
  // Hàm để chọn phòng
  selectRoom(room: any) {
    this.selectedRoom = room;  // Cập nhật phòng được chọn
  }
  // Xử lý sự kiện nhấp chuột phải (context menu)
  onRightClick(event: MouseEvent, room: any) {
    event.preventDefault();  // Ngăn trình duyệt hiển thị menu mặc định
    this.selectedRoom = room;
    this.showMenu = true;
    this.menuPosition = {
      top: `${event.clientY}px`,
      left: `${event.clientX}px`
    };
  }

  // Ẩn menu khi nhấp vào chỗ khác
  onClickOutside(event: MouseEvent) {
    if (!this.showMenu) return;
    const menu = document.querySelector('.context-menu');
    if (menu && !menu.contains(event.target as Node)) {
      this.showMenu = false;
    }
  }

  joinRoom(room: any) {
    if (!room.status) {
      alert('Phòng này đang chơi, không thể vào');
      return;
    }
  
    // Kiểm tra nếu playerId và password không phải là null
    if (!this.profile || !this.profile.id ) {
      alert('Cần có đầy đủ thông tin người chơi và mật khẩu');
      return;
    }
  
   
    const requestBody = {
      roomId: room.id,
      userId: this.profile.id,
    };
    console.log(requestBody);  // Kiểm tra các giá trị trong form
    // // Gửi yêu cầu tham gia phòng
    // this.SharedService.joinRoom(requestBody).subscribe({
    //   next: (response) => {
    //     console.log('Tham gia phòng thành công:', response);
    //     localStorage.setItem(`room_${this.profile.id}`, room.id);
    //     this.router.navigate(['/game']);  // Điều hướng đến trang game
    //   },
    //   error: (error) => {
    //     console.error('Lỗi khi tham gia phòng:', error);
    //     alert('Có lỗi xảy ra khi tham gia phòng');
    //   }
    // });
    this.SharedService.joinRoom(requestBody).subscribe({
      next: async (response) => {
        console.log('Tham gia phòng thành công:', response);
        localStorage.setItem(`room_${this.profile.id}`, room.id);
  
          // ✅ Gọi hàm join SignalR từ SharedService
        await this.SharedService.joinSignalRRoom(room.id);
        // Điều hướng tới trang game
        this.router.navigate(['/game']);
      },
      error: (error) => {
        console.error('Lỗi khi tham gia phòng:', error);
        alert('Có lỗi xảy ra khi tham gia phòng');
      }
    });
  }
  
  
  // Tải thông tin profile của người dùng sau khi component khởi tạo
  loadProfile() {
    this.SharedService.getProfile().subscribe({
      next: (res) => {
        this.profile = res;
      },
      error: (err) => {
        console.error("Lỗi khi tải hồ sơ", err);
      }
    });
  }

  ngOnInit(): void {
    this.loadProfile();  // Gọi hàm loadProfile khi component được khởi tạo
    this.layDSroom();
    
  }
}
