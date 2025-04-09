
import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'caro_font';
  constructor(private router: Router) {}

  goToRoom() {
   
    this.router.navigate(['/room']); // Điều hướng tới trang room
  }
  goToJoinRoom() {
    this.router.navigate(['/joinroom']);
  }
  goToRankUser() {
    this.router.navigate(['/rankUser']);
  }
  goToPlayGame() {
    this.router.navigate(['/game']);
  }

  goToRankGame() {
    this.router.navigate(['/rankgame']);
  }
}
