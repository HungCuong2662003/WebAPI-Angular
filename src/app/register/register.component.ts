import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SharedService } from 'src/app/shared.service';
import { Router } from '@angular/router'; // Đảm bảo sử dụng Router ở đây

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  user = { Firstname: '', Lastname: '', Email: '', Password: '', ConfirmPass: '' };
  message = '';

  constructor(private SharedService: SharedService, private router: Router) {}

  signUp() {
    if (this.user.Password !== this.user.ConfirmPass) {
      this.message = 'Mật khẩu không khớp!';
      return;
    }

    this.SharedService.signUp(this.user).subscribe({
      next: () => {
        this.message = 'Đăng ký thành công! Hãy đăng nhập.';
        setTimeout(() => this.router.navigate(['/login']), 2000);
      },
      error: (err) => {
        this.message = 'Đăng ký thất bại!';
        console.error(err);
      },
    });
  }
}