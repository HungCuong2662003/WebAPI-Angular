import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/shared.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-create-room',
  templateUrl: './create-room.component.html',
  styleUrls: ['./create-room.component.scss']
})
export class CreateRoomComponent implements OnInit {
  roomForm: FormGroup;
  loading: boolean = false;
  profile: any;
  createdRoomId: number | null = null;  // Biến để hiển thị room ID

  constructor(
    private fb: FormBuilder,
    private SharedService: SharedService,
    private router: Router
  ) {
    this.roomForm = this.fb.group({
      roomCode: ['', [Validators.required, Validators.maxLength(10)]],
      passwordRoom: [''],
      isPublic: [false, Validators.required],
      status: [true, Validators.required],
      ownerID: ['']  // Thêm ownerID vào form để nhận giá trị từ profile
    });
  }

  // Hàm thay đổi giữa phòng công khai và riêng tư
  onPublicChange(event: any) {
    if (event.target.checked) {
      this.roomForm.get('passwordRoom')?.clearValidators();
      this.roomForm.get('passwordRoom')?.setValue('');
    } else {
      this.roomForm.get('passwordRoom')?.setValidators([Validators.required]);
    }
    this.roomForm.get('passwordRoom')?.updateValueAndValidity();
  }

  // Hàm submit form để thêm phòng
  onSubmit() {
    if (this.roomForm.valid) {
      this.loading = true;

      // Truyền đúng ownerID vào form nếu chưa cập nhật
      if (!this.roomForm.get('ownerID')?.value) {
        this.roomForm.get('ownerID')?.setValue(this.profile.id);  // Cập nhật ownerID vào form
      }

      console.log(this.roomForm.value);  // Kiểm tra các giá trị trong form

      this.SharedService.addRoom(this.roomForm.value).subscribe({
        next: (response) => {
          this.loading = false;
          this.createdRoomId = response.id;
          // this.router.navigate(['/room']);  // Điều hướng đến trang danh sách phòng
        },
        error: (error) => {
          this.loading = false;
          console.error('Lỗi khi tạo phòng:', error);
        }
      });
    }
  }

  // Tải thông tin profile của người dùng sau khi component khởi tạo
  loadProfile() {
    this.SharedService.getProfile().subscribe({
      next: (res) => {
        this.profile = res;
        // Cập nhật ownerID trong form sau khi tải profile
        this.roomForm.get('ownerID')?.setValue(this.profile.id);  // Tự động điền ownerID
      },
      error: (err) => {
        console.error("Lỗi khi tải hồ sơ", err);
      }
    });
  }
  copyToClipboard(id: number) {
    navigator.clipboard.writeText(id.toString()).then(() => {
      alert("Đã sao chép Room ID!");
    }).catch(err => {
      console.error('Lỗi khi sao chép:', err);
    });
  }

  ngOnInit(): void {
    this.loadProfile();  // Gọi hàm loadProfile khi component được khởi tạo
  }
}
