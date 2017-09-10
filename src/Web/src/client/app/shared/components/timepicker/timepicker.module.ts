import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TimePickerComponent } from './timepicker.component';
@NgModule({
  imports: [
    CommonModule],
  declarations: [TimePickerComponent],
  exports: [TimePickerComponent]
})
export class TimePickerModule {
}
