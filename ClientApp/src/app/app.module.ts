import { PickBannedCardsComponent } from './_components/_modals/pick-banned-cards/pick-banned-cards.component';
import { PickNumbersToDiscardComponent } from './_components/_modals/pick-numbers-to-discard/pick-numbers-to-discard.component';
import { BlackjackComponent } from './_components/_modals/Blackjack/Blackjack.component';
import { PickCharityCardsComponent } from './_components/_modals/pick-charity-cards/pick-charity-cards.component';
import { PickDuelNumbersComponent } from './_components/_modals/pick-duel-numbers/pick-duel-numbers.component';
import { GameInfoComponent } from './_components/_modals/game-info/game-info.component';
import { HelpComponent } from './_components/help/help.component';
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
import { DigCardComponent } from './_components/_modals/dig-card/dig-card.component';

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
    ShowHandComponent,
    DigCardComponent,
    HelpComponent,
    GameInfoComponent,
    BlackjackComponent,
    PickCharityCardsComponent,
    PickNumbersToDiscardComponent,
    PickBannedCardsComponent,
    PickDuelNumbersComponent
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
      { path: 'help', component: HelpComponent },
      { path: '**', redirectTo: '/' }
    ])
  ],
  providers: [HubService, WaitingRoomGuard, WaitingRoomDeactivateGuard, GameGuard, GameDeactivateGuard],
  bootstrap: [AppComponent],
  entryComponents: [PickColorComponent, PickBannedCardsComponent,PickPlayerComponent, ShowHandComponent, DigCardComponent, GameInfoComponent,PickDuelNumbersComponent, PickNumbersToDiscardComponent,PickCharityCardsComponent, BlackjackComponent]
})
export class AppModule {}
