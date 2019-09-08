/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { GameEndedResultComponent } from './game-ended-result.component';

describe('GameEndedResultComponent', () => {
  let component: GameEndedResultComponent;
  let fixture: ComponentFixture<GameEndedResultComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ GameEndedResultComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GameEndedResultComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
