import { PickPromiseCardComponent } from './_components/_modals/pick-promise-card/pick-promise-card.component';
import { ChangeLogComponent } from './_components/change-log/change-log.component';
import { SidebarSettingsComponent } from './_components/sidebar-settings/sidebar-settings.component';
import { UtilityService } from './_services/utility.service';
import { PickBannedCardsComponent } from './_components/_modals/pick-banned-cards/pick-banned-cards.component';
import { PickNumbersToDiscardComponent } from './_components/_modals/pick-numbers-to-discard/pick-numbers-to-discard.component';
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
import { MentionModule } from 'angular-mentions';
import { AppComponent } from './app.component';
import { HomeComponent } from './_components/home/home.component';
import { OnlinePlayersComponent } from './_components/online-players/online-players.component';
import { WaitingRoomGuard } from './_guards/waiting-room.guard';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { WaitingRoomDeactivateGuard } from './_guards/waiting-room-deactivate.guard';
import { PickColorComponent } from './_components/_modals/pick-color/pick-color.component';
import { DigCardComponent } from './_components/_modals/dig-card/dig-card.component';
import { BlackjackComponent } from './_components/_modals/blackjack/blackjack.component';
import { DividePerCapitalPipe } from './_pipes/divide-per-capital.pipe';
import { ShowCardsComponent } from './_components/_modals/show-cards/show-cards.component';
import { NavbarComponent } from './_components/navbar/navbar.component';
import { UserSettingsComponent } from './_components/_modals/user-settings/user-settings.component';
import { GuessOddEvenNumberComponent } from './_components/_modals/guess-odd-even-number/guess-odd-even-number.component';
import { GameSetupComponent } from './_components/_modals/game-setup/game-setup.component';

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
    ShowCardsComponent,
    DigCardComponent,
    HelpComponent,
    GameInfoComponent,
    ChangeLogComponent,
    BlackjackComponent,
    PickCharityCardsComponent,
    PickNumbersToDiscardComponent,
    PickBannedCardsComponent,
    PickDuelNumbersComponent,
    SidebarSettingsComponent,
    NavbarComponent,
    UserSettingsComponent,
    PickPromiseCardComponent,
    DividePerCapitalPipe,
    GuessOddEvenNumberComponent,
    GameSetupComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BrowserAnimationsModule,
    ToastrModule.forRoot({
      preventDuplicates: true,
      timeOut: 3000
    }),
    HttpClientModule,
    FormsModule,
    SidebarModule.forRoot(),
    NgbModule,
    MentionModule,
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
      { path: 'change-log', component: ChangeLogComponent },
      { path: '**', redirectTo: '/' }
    ])
  ],
  providers: [HubService, WaitingRoomGuard, WaitingRoomDeactivateGuard, GameGuard, GameDeactivateGuard, UtilityService],
  bootstrap: [AppComponent],
  entryComponents: [
    PickColorComponent,
    PickBannedCardsComponent,
    PickPlayerComponent,
    ShowCardsComponent,
    DigCardComponent,
    GameInfoComponent,
    PickDuelNumbersComponent,
    PickNumbersToDiscardComponent,
    PickCharityCardsComponent,
    BlackjackComponent,
    UserSettingsComponent,
    PickPromiseCardComponent,
    GuessOddEvenNumberComponent,
    GameSetupComponent
  ]
})
export class AppModule {}
