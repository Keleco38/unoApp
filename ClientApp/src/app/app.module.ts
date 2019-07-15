import { PickPlayerComponent } from './_components/_modals/pick-player/pick-player.component';
import { GameDeactivateGuard } from './_guards/game-deactivate.guard';
import { GameGuard } from './_guards/game.guard';
import { GameComponent } from './_components/game/game.component';
import { GameChatComponent } from './_components/game-chat/game-chat.component';
import { GameSpectatorsComponent } from './_components/game-spectators/game-spectators.component';
import { GameTabsComponent } from './_components/game-tabs/game-tabs.component';
import { WaitingRoomComponent } from './_components/waiting-room/waiting-room.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AvailableGamesComponent } from './_components/available-games/available-games.component';
import { AllChatComponent } from './_components/all-chat/all-chat.component';
import { HubService } from './_services/hub.service';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { ToastrModule } from 'ngx-toastr';
import { SidebarModule } from 'ng-sidebar';

import { AppComponent } from './app.component';
import { HomeComponent } from './_components/home/home.component';
import { OnlinePlayersComponent } from './_components/online-players/online-players.component';
import { WaitingRoomGuard } from './_guards/waiting-room.guard';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { WaitingRoomDeactivateGuard } from './_guards/waiting-room-deactivate.guard';
import { PickColorComponent } from './_components/_modals/pick-color/pick-color.component';
import { ShowHandComponent } from './_components/_modals/show-hand/show-hand.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    AvailableGamesComponent,
    OnlinePlayersComponent,
    AllChatComponent,
    WaitingRoomComponent,
    GameTabsComponent,
    GameSpectatorsComponent,
    GameChatComponent,
    PickColorComponent,
    GameComponent,
    PickPlayerComponent,
    ShowHandComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BrowserAnimationsModule,
    ToastrModule.forRoot(),
    HttpClientModule,
    FormsModule,
    SidebarModule.forRoot(),
    NgbModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      {
        path: 'waitingRoom',
        component: WaitingRoomComponent,
        canDeactivate: [WaitingRoomDeactivateGuard],
        canActivate: [WaitingRoomGuard]
      },
      { path: 'game', component: GameComponent, canActivate: [GameGuard], canDeactivate: [GameDeactivateGuard] },
      { path: '**', redirectTo: '/' }
    ])
  ],
  providers: [HubService, WaitingRoomGuard, WaitingRoomDeactivateGuard, GameGuard, GameDeactivateGuard],
  bootstrap: [AppComponent],
  entryComponents: [PickColorComponent, PickPlayerComponent, ShowHandComponent]
})
export class AppModule {}
