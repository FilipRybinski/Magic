import { Component, OnInit } from '@angular/core';
import {FormBuilder, FormGroup,FormGroupDirective,Validators} from '@angular/forms';
import { matchValidator } from '../shared/validators/confirmPasswordValidtor';
import {Countries} from '../shared/models/country.model'
import { countries } from '../shared/models/countryData.store';
import { ApiService } from '../shared/api.service';
import { user } from '../shared/models/user.models';
@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit{
  registerForm!:FormGroup;
  ErrorMessage!:string[];
  countriesStore:Countries[]=countries;
  constructor(private formBuilder:FormBuilder,private api:ApiService){}
  ngOnInit(): void {
    this.registerForm=this.formBuilder.group({
      userName:['',Validators.compose([Validators.required,Validators.maxLength(15)])],
      userPassword:['',Validators.compose([Validators.required,Validators.pattern('(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&].{8,}')]),,matchValidator('userPassword',true)],
      userPasswordConfirmed:['',Validators.compose([Validators.required,matchValidator('userPassword')])],
      email:['',Validators.compose([Validators.required,Validators.email])],
      postalDetails: this.formBuilder.group({
        city:['',Validators.compose([Validators.required,Validators.maxLength(15)])],
        country:['',[Validators.required]],
        postalCode:['',Validators.compose([Validators.required,Validators.pattern('^[0-9]{2,5}(:|\-)?[0-9]{3,4}')])],
        street:['',Validators.compose([Validators.required,Validators.maxLength(15)])]
      }),
      personalData:this.formBuilder.group({
        name:['',Validators.compose([Validators.required,Validators.maxLength(15)])],
        surname:['',Validators.compose([Validators.required,Validators.maxLength(15)])],
        phoneNumber:['',Validators.compose([Validators.required,Validators.minLength(9)])]
      })
    })
  }
  onSubmit(form:FormGroupDirective){
    if(!this.registerForm.valid){
      return;
    }
    var user:user={
      userName:this.registerForm.value.userName,
      userPassword:this.registerForm.value.userPassword,
      userPasswordConfirmed:this.registerForm.value.userPasswordConfirmed,
      email:this.registerForm.value.email,
      postalDetails:this.registerForm.value.postalDetails,
      personalData:this.registerForm.value.personalData
    }
    this.api.postNewUser(user).subscribe(
      respone=>{
      },
      error=>{
        this.ErrorMessage=[];
        this.ErrorMessage=Object.values(error.error.errors);
        form.resetForm(user);
      }
    )
  }
}
