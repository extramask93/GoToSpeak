import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'utcStringPipe'
})
export class UtcStringPipePipe implements PipeTransform {

  transform(value: string, args?: any): Date {

      if (value.indexOf('Z') === -1 && value.indexOf('+') === -1) {
          value += 'Z';
      }
      return new Date(value);
  }

}
