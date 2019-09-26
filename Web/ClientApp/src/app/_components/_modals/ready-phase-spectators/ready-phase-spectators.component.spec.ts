/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { ReadyPhaseSpectatorsComponent } from './ready-phase-spectators.component';

describe('ReadyPhaseSpectatorsComponent', () => {
  let component: ReadyPhaseSpectatorsComponent;
  let fixture: ComponentFixture<ReadyPhaseSpectatorsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ReadyPhaseSpectatorsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReadyPhaseSpectatorsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
