import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RankgameComponent } from './rankgame.component';

describe('RankgameComponent', () => {
  let component: RankgameComponent;
  let fixture: ComponentFixture<RankgameComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RankgameComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RankgameComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
