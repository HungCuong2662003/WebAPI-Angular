

import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/shared.service';

@Component({
  selector: 'app-user',
  templateUrl: './rankUser.component.html',
  styleUrls: ['./rankUser.component.scss']
})
export class rankUser implements OnInit {
  DSUser: any[] = [];

  constructor(private service: SharedService) {}

  ngOnInit(): void {
    this.loadDSUser();
  }

  loadDSUser(): void {
    this.service.layDSUser().subscribe({
      next: (data) => {
       
        this.DSUser = (data as any)?.$values || [];

      },
      error: (err) => {
        console.error('Error loading data:', err);
      }
    });
  }
}
