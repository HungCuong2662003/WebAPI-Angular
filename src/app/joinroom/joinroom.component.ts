import { Component, OnInit } from '@angular/core';
import { SharedService } from '../shared.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-joinroom',
  templateUrl: './joinroom.component.html',
  styleUrls: ['./joinroom.component.scss']
})
export class JoinroomComponent implements OnInit {

  roomId: string = '';
  password: string = '';
  profile: any;
  message: string = '';

  constructor(private SharedService: SharedService, private router: Router) {}

  ngOnInit(): void {
    this.loadProfile();
  }

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

  joinRoom() {
    if (!this.profile || !this.profile.id) {
      this.message = 'Bạn cần đăng nhập trước khi tham gia phòng.';
      return;
    }

    if (!this.roomId || !this.password) {
      this.message = 'Vui lòng nhập đầy đủ Room ID và Mật khẩu.';
      return;
    }

    const requestBody = {
      roomId: this.roomId,
      userID: this.profile.id
    };
console.log(requestBody)
    this.SharedService.joinRoomWithPassword(requestBody, this.password).subscribe({
      next: async (response) => {
        console.log('Tham gia phòng thành công:', response);
        localStorage.setItem(`room_${this.profile.id}`, this.roomId);

        await this.SharedService.joinSignalRRoom(this.roomId);
        this.router.navigate(['/game']);
      },
      error: (error) => {
        console.error('Lỗi khi tham gia phòng:', error);
        this.message = error.error?.message || 'Không thể tham gia phòng.';
      }
    });
  }
}
