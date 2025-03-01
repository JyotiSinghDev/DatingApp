import { HttpClient } from '@angular/common/http';
import { Component, EventEmitter, inject, Input, input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  accountService = inject(AccountService);
  toastr = inject(ToastrService)
  model: any={};
  @Input() usersFromHomeComponent:any;
  @Output() cancelRegister = new EventEmitter();

  submitRegistration() {
    this.accountService.registration(this.model).subscribe({
      next:response=>{
        console.log(response);
        this.cancel();
      },
      error:error=>{this.toastr.error(error.error)}
    })
  }
  cancel() {
    this.cancelRegister.emit(true);
  }
}
