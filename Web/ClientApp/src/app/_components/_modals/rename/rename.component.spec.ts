/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { RenameComponent } from './rename.component';

describe('RenameComponent', () => {
  let component: RenameComponent;
  let fixture: ComponentFixture<RenameComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RenameComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RenameComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
