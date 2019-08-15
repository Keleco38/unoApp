import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'dividePerCapital'
})
export class DividePerCapitalPipe implements PipeTransform {

  transform(value: string, args?: any): string {
    return value.match(/([A-Z]?[^A-Z]*)/g).slice(0,-1).join(" ").toLowerCase();
  }

}
