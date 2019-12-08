/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { PickWildCardComponent } from './pick-wild-card.component';

describe('PickWildCardComponent', () => {
  let component: PickWildCardComponent;
  let fixture: ComponentFixture<PickWildCardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PickWildCardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PickWildCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
