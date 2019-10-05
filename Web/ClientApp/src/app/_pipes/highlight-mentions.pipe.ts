import { Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

@Pipe({
  name: 'highlightMentions'
})
export class HighlightMentionsPipe implements PipeTransform {
  constructor(private _domSanitizer: DomSanitizer) {}

  transform(text: any, args?: any): any {
    return this._domSanitizer.bypassSecurityTrustHtml(this.stylize(text));
  }

  private stylize(text: string): string {
    let stylizedText: string = '';
    if (text && text.length > 0) {
      for (let t of text.split(' ')) {
        if (t.startsWith('@') && t.length > 1) stylizedText += `<mark>${t}</mark> `;
        else stylizedText += t + ' ';
      }
      return stylizedText;
    }
  }
}
