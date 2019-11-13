import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'utcPipe'
})
export class UtcPipePipe implements PipeTransform {

  transform(value: Date, args?: any): Date {
              // append 'Z' to the date string to indicate UTC time if the timezone isn't already specified
              let str = value.toString();
              if (str.indexOf('Z') === -1 && str.indexOf('+') === -1) {
                  str += 'Z';
              }
              return new Date(str);
          }
}
