
import { Component, OnInit } from '@angular/core';
import { SharedService } from '../shared.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  user = { Email: '', Password: '' };
  message = '';

  constructor(private SharedService: SharedService, private router: Router) {}

  signIn() {
    this.SharedService.signIn(this.user).subscribe({
      next: (res) => {
        this.message = 'Đăng nhập thành công!';
        localStorage.setItem('token', res.token);
  
         this.router.navigate(['/profile']); // Chuyển đến trang cá nhân
      },
      error: () => {
        this.message = 'Sai thông tin đăng nhập!';
      },
    });
  }
  goToRegister() {
    this.router.navigate(['/register']); // Điều hướng tới trang tạo phòng
  }
}


