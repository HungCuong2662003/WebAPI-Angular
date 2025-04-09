import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { rankUser } from './RankUser/rankUser.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { RankgameComponent } from './rankgame/rankgame.component';
import { RoomComponent } from './room/room.component';
import { CreateRoomComponent } from './room/create-room/create-room.component';
import { GameComponent } from './game/game.component';
import { ProfileComponent } from './profile/profile.component';
import { JoinroomComponent } from './joinroom/joinroom.component';
const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  {path:'rankUser', component:rankUser},
  {path:'login', component:LoginComponent},
  {path:'register', component:RegisterComponent},
  { path: 'room', component: RoomComponent }, 
  { path: 'createroom', component: CreateRoomComponent },
  { path: 'rankgame', component: RankgameComponent },
  { path: 'game', component: GameComponent },
  { path: 'profile', component: ProfileComponent },
  { path: 'joinroom', component: JoinroomComponent },

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
