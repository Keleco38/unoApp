import { HighlightMentionsPipe } from './_pipes/highlight-mentions.pipe';
import { ModalSubscribingService } from './_services/modal-subscribing.service';
import { KickBanPlayerComponent } from './_components/_modals/kick-ban-player/kick-ban-player.component';
import { UserStorageService } from './_services/storage-services/user-storage.service';
import { TournamentStorageService } from './_services/storage-services/tournament-storage.service';
import { LobbyStorageService } from './_services/storage-services/lobby-storage.service';
import { ReadyPhaseSpectatorsComponent } from './_components/_modals/ready-phase-spectators/ready-phase-spectators.component';
import { ConfirmReadyComponent } from './_components/_modals/confirm-ready/confirm-ready.component';
import { FirstTimeLaunchComponent } from './_components/_modals/first-time-launch/first-time-launch.component';
import { TournamentWinnersComponent } from './_components/tournament-winners/tournament-winners.component';
import { AdminSectionComponent } from './_components/_modals/admin-section/admin-section.component';
import { ContactFormComponent } from './_components/_modals/contact-form/contact-form.component';
import { TournamentSetupComponent } from './_components/_modals/tournament-setup/tournament-setup.component';
import { TournamentChatComponent } from './_components/tournament-chat/tournament-chat.component';
import { TournamentSpectatorsComponent } from './_components/tournament-spectators/tournament-spectators.component';
import { HttpService } from './_services/http.service';
import { HallOfFameComponent } from './_components/hall-of-fame/hall-of-fame.component';
import { GameEndedResultComponent } from './_components/_modals/game-ended-result/game-ended-result.component';
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
import { AvailableTournamentsComponent } from './_components/available-tournaments/available-tournaments.component';
import { TournamentWaitingRoomComponent } from './_components/tournament-waiting-room/tournament-waiting-room.component';
import { TournamentComponent } from './_components/tournament/tournament.component';
import { TournamentDeactivateGuard } from './_guards/tournament-deactivate.guard';
import { TournamentGuard } from './_guards/tournament.guard';
import { TournamentWaitingRoomDeactivateGuard } from './_guards/tournament-waiting-room-deactivate.guard';
import { TournamentWaitingRoomGuard } from './_guards/tournament-waiting-room.guard';
import { UiSwitchModule } from 'ngx-toggle-switch';
import { RenameComponent } from './_components/_modals/rename/rename.component';
import { ModalService } from './_services/modal.service';
import { GameStorageService } from './_services/storage-services/game-storage.service';
import { InputPasswordComponent } from './_components/_modals/input-password/input-password.component';
import { NgxLinkifyjsModule } from 'ngx-linkifyjs';

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
    HighlightMentionsPipe,
    TournamentSpectatorsComponent,
    TournamentChatComponent,
    HallOfFameComponent,
    GuessOddEvenNumberComponent,
    GameEndedResultComponent,
    AvailableTournamentsComponent,
    GameSetupComponent,
    TournamentSetupComponent,
    ContactFormComponent,
    TournamentWaitingRoomComponent,
    AdminSectionComponent,
    FirstTimeLaunchComponent,
    ReadyPhaseSpectatorsComponent,
    ConfirmReadyComponent,
    TournamentWinnersComponent,
    RenameComponent,
    KickBanPlayerComponent,
    InputPasswordComponent,
    TournamentComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    UiSwitchModule,
    BrowserAnimationsModule,
    NgxLinkifyjsModule.forRoot(),
    ToastrModule.forRoot({
      preventDuplicates: true,
      resetTimeoutOnDuplicate:true,
      positionClass: 'toast-bottom-right',
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
        path: 'waiting-room',
        component: WaitingRoomComponent,
        canDeactivate: [WaitingRoomDeactivateGuard],
        canActivate: [WaitingRoomGuard]
      },
      { path: 'game', component: GameComponent, canActivate: [GameGuard], canDeactivate: [GameDeactivateGuard] },
      { path: 'help', component: HelpComponent },
      { path: 'change-log', component: ChangeLogComponent },
      { path: 'tournament-winners', component: TournamentWinnersComponent },
      { path: 'hall-of-fame', component: HallOfFameComponent },
      {
        path: 'tournament-waiting-room',
        component: TournamentWaitingRoomComponent,
        canDeactivate: [TournamentWaitingRoomDeactivateGuard],
        canActivate: [TournamentWaitingRoomGuard]
      },
      { path: 'tournament', component: TournamentComponent, canDeactivate: [TournamentDeactivateGuard], canActivate: [TournamentGuard] },
      { path: '**', redirectTo: '/' }
    ])
  ],
  providers: [
    HubService,
    WaitingRoomGuard,
    TournamentWaitingRoomGuard,
    TournamentWaitingRoomDeactivateGuard,
    WaitingRoomDeactivateGuard,
    GameGuard,
    GameDeactivateGuard,
    TournamentGuard,
    TournamentDeactivateGuard,
    UtilityService,
    ModalSubscribingService,
    ModalService,
    GameStorageService,
    LobbyStorageService,
    TournamentStorageService,
    UserStorageService,
    HttpService
  ],
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
    FirstTimeLaunchComponent,
    UserSettingsComponent,
    PickPromiseCardComponent,
    ContactFormComponent,
    GuessOddEvenNumberComponent,
    GameSetupComponent,
    GameEndedResultComponent,
    RenameComponent,
    ConfirmReadyComponent,
    TournamentSetupComponent,
    ReadyPhaseSpectatorsComponent,
    KickBanPlayerComponent,
    InputPasswordComponent,
    AdminSectionComponent
  ]
})
export class AppModule {}
