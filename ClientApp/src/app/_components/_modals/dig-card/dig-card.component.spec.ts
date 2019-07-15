/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { DigCardComponent } from './dig-card.component';

describe('DigCardComponent', () => {
  let component: DigCardComponent;
  let fixture: ComponentFixture<DigCardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DigCardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DigCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
