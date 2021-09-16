import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { throwError } from 'rxjs';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  @Output() cancelRegister = new EventEmitter();
  registerForm: FormGroup;
  maxDate:Date;
  validationErrors: string[]=[];


  constructor(private accountService: AccountService,
    private router:Router, private toastr: ToastrService,
      private fb :FormBuilder  ) { }

  ngOnInit(): void {
    this.intializaFrom();
    this.maxDate=new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear() -18);
  }

  intializaFrom() {
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]]
    })
  }


  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      if (control.parent && control.parent.controls) {
        return control.value === (control.parent.controls as { [key: string]: AbstractControl })[matchTo].value
          ? null : { isMatching: true };
      }
      return null;
    }
  }




  register() {
    this.accountService.register(this.registerForm.value).subscribe(response=>{
      this.router.navigateByUrl('/members');
      this.cancel(); 
    },error=>{
      this.validationErrors=error;
    })
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
