import { Subscription } from 'rxjs';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';

@Component({
  selector: 'app-help',
  templateUrl: './help.component.html',
  styleUrls: ['./help.component.css']
})
export class HelpComponent implements OnInit, OnDestroy {
  private _subscription: Subscription;
  constructor(private _router: Router) {}

  ngOnInit() {
    this._subscription = this._router.events.subscribe(s => {
      if (s instanceof NavigationEnd) {
        const tree = this._router.parseUrl(this._router.url);
        if (tree.fragment) {
          const element = document.querySelector('#' + tree.fragment);
          if (element) {
            element.scrollIntoView(true);
          }
        }
      }
    });
  }

  ngOnDestroy(): void {
    this._subscription.unsubscribe();
  }
}
