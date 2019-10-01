import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { GameEndedResult } from 'src/app/_models/gameEndedResult';
import { KeyValue } from '@angular/common';
import { Card } from 'src/app/_models/card';

@Injectable({
  providedIn: 'root'
})
export class ModalObservablesService {
  readyPhasePlayersModalObservable = new Subject<boolean>();
  readyPhaseSpectatorsModalObservable = new Subject<boolean>();
  gameEndedResultModalObservable = new Subject<GameEndedResult>();
  showCardsModalObservable = new Subject<KeyValue<string, Card[]>[]>();
  renameModalObservable = new Subject<boolean>();

  constructor() {}
}
