import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'highlightMentions'
})
export class HighlightMentionsPipe implements PipeTransform {
  transform(text: any, args?: any): any {
    let stylizedText: string = '';
    if (text && text.length > 0) {
      for (let t of text.split(' ')) {
        if (t.startsWith('@') && t.length > 1) stylizedText += `<mark>${t}</mark> `;
        else stylizedText += t + ' ';
      }
      return stylizedText;
    } else return text;
  }
}
