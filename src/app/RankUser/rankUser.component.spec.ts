import { ComponentFixture, TestBed } from '@angular/core/testing';

import { rankUser } from './rankUser.component';

describe('UserComponent', () => {
  let component: rankUser;
  let fixture: ComponentFixture<rankUser>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ rankUser ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(rankUser);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
