import { HttpClient } from '@angular/common/http';
import { Component, EventEmitter, inject, Input, input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { JsonPipe, NgIf } from '@angular/common';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule,NgIf,BsDatepickerModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit{
  private accountService = inject(AccountService);
  private toastr = inject(ToastrService);
  private fb = inject(FormBuilder);
  private router = inject(Router);
  validationErrors :string[]|undefined;
  registerForm: FormGroup = new FormGroup({});
  @Input() usersFromHomeComponent:any;
  @Output() cancelRegister = new EventEmitter();


  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm(){
    this.registerForm= this.fb.group({
      gender:['male'],
      username: ['',Validators.required],
      knownAs:['',Validators.required],
      dateOfBirth:['',Validators.required],
      city:['',Validators.required],
      country:['',Validators.required],
      password: ['',[Validators.required,Validators.minLength(4),Validators.maxLength(8)]],
      confirmPassword: ['',[Validators.required,this.matchValues('password')]]
    });
    this.registerForm.controls['password'].valueChanges.subscribe({
      next:()=>this.registerForm.controls['confirmPassword'].updateValueAndValidity()
    })
  }
 

  matchValues(matchTo:string):ValidatorFn{
    return (control:AbstractControl)=>{
      return control.value==control.parent?.get(matchTo)?.value?null:{isMatching:true}
    }
  }

  submitRegistration() {
    const dob = this.getDateOnly(this.registerForm.get('dateOfBirth')?.value);
    this.registerForm.patchValue({dateOfBirth:dob});
    this.accountService.registration(this.registerForm.value).subscribe({
      next:_=>this.router.navigateByUrl('/members'),
      error:error=>this.validationErrors =error
    })
  }
  cancel() {
    this.cancelRegister.emit(true);
  }
  private getDateOnly(dob:string | undefined){
    if(!dob) return;
    return new Date(dob).toISOString().slice(0,10);
  }
}
