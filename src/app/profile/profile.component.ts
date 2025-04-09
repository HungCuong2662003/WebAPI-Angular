
import { Component, OnInit } from '@angular/core';
import { SharedService } from '../shared.service';
import { Router } from '@angular/router';
@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  profile: any;

  constructor(private sharedService: SharedService, private router: Router) {}

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile() {
    this.sharedService.getProfile().subscribe({
      next: (res) => {
        localStorage.setItem("UserId", res.id)
        this.profile = res;
      },
      error: (err) => {
        console.error("Lỗi khi tải hồ sơ", err);
      }
    });
  }
    // Hàm đăng xuất
     // Hàm đăng xuất
  logout() {
    this.sharedService.logout();  // Gọi phương thức logout để xóa token khỏi localStorage
    this.router.navigate(['/login']);  // Điều hướng người dùng về trang đăng nhập
  }
}
