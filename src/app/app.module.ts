import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { LoginComponent } from './login/login.component';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { rankUser } from './RankUser/rankUser.component';
import { RoomComponent } from './room/room.component';
import { CreateRoomComponent } from './room/create-room/create-room.component';
import { RegisterComponent } from './register/register.component';
import { RankgameComponent } from './rankgame/rankgame.component';
import { GameComponent } from './game/game.component';
import { ProfileComponent } from './profile/profile.component';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule ,ReactiveFormsModule} from '@angular/forms';
import { JoinroomComponent } from './joinroom/joinroom.component';

@NgModule({
  declarations: [
    AppComponent,
    rankUser,
    RoomComponent,
    CreateRoomComponent,
    RegisterComponent,
    RankgameComponent,
    GameComponent,
    ProfileComponent,
    LoginComponent,
    JoinroomComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule, ReactiveFormsModule

  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
