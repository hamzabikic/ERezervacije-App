import { Component, OnInit } from '@angular/core';
import {AutentifikacijaEndpoints} from "../Endpoints/AutentifikacijaEndpoints";
import {Grad, RegistracijaRequest} from "../AutentifikacijaKlase/LoginRequest";
import {Router} from "@angular/router";
import {isNullOrEmpty} from "../Helpers/StringHelper";

@Component({
  selector: 'app-registracija',
  templateUrl: './registracija.component.html',
  styleUrls: ['./registracija.component.css']
})
export class RegistracijaComponent implements OnInit {

  constructor(private endpoints:AutentifikacijaEndpoints, private router:Router) { }
  listaGradova: Grad[] = [];
  ime="";
  prezime ="";
  datumRodjenja:Date = new Date();
  email="";
  username="";
  password ="";
  brojTelefona ="";
  greska ="";
  grad:number=1;

  ngOnInit(): void {
     this.endpoints.GetGradovi().subscribe(res=> this.listaGradova = res);
  }
  provjeraPolja(event:Event) {
    let element = event.target as HTMLInputElement;
    if(isNullOrEmpty(element.value)){
      element.style.border="1px solid red";
      this.greska="Sva polja su obavezna!";
    }
    else {
      element.style.border="1px solid rgb(128, 128, 128)";
      this.greska="";
    }
  }
  provjeriPolja () {
    if(isNullOrEmpty(this.ime) || isNullOrEmpty(this.prezime) || isNullOrEmpty(this.brojTelefona)
      || isNullOrEmpty(this.username) || isNullOrEmpty(this.email) || isNullOrEmpty(this.password) ||this.grad==0)
    {
      this.greska="Sva polja su obavezna!";
      return true;
    }
    this.greska="";
    return false;

  }
  async registracija() {
  if(this.provjeriPolja()) return;
  if(await this.provjeraUsername() || await this.provjeraEmaila() || await this.provjeraTelefona()) {
    return;
  }
  let regrequest:RegistracijaRequest = {
    ime :this.ime,
    prezime: this.prezime,
    datumRodjenja:this.datumRodjenja,
    email: this.email,
    username:this.username,
    password:this.password,
    brojTelefona:this.brojTelefona,
    gradID: this.grad,
    aktivan:true
  };
   let regresponse = await this.endpoints.Registracija(regrequest).toPromise();
   if(regresponse?.registrovan) {
     this.greska="";
     this.router.navigate(["/login"]);
   }
   else {
     this.greska=regresponse!.greska;
   }

  }

  async provjeraUsername() {
   let postoji = await this.endpoints.PostojiUsername(this.username).toPromise();
   if(postoji?.postoji) {
     this.greska= "Uneseni username se vec koristi!";

     // @ts-ignore
     document.getElementById("username").style.border="1px solid red";
     return true;
   }
    this.greska= "";
    // @ts-ignore
    document.getElementById("username").style.border="1px solid rgb(128, 128, 128)";
   return false;
  }

  async  provjeraTelefona() {
    let postoji = await this.endpoints.PostojiBrojTelefona(encodeURIComponent(this.brojTelefona)).toPromise();
    if(postoji?.postoji) {
      this.greska= "Uneseni broj telefona se vec koristi!";

      // @ts-ignore
      document.getElementById("brojTelefona").style.border="1px solid red";
      return true;
    }
    this.greska= "";
    // @ts-ignore
    document.getElementById("brojTelefona").style.border="1px solid rgb(128, 128, 128)";
    return false;
  }

  async provjeraEmaila() {
    let postoji = await this.endpoints.PostojiEmail(this.email).toPromise();
    if(postoji?.postoji) {
      this.greska= "Uneseni email se vec koristi!";

      // @ts-ignore
      document.getElementById("email").style.border="1px solid red";
      return true;
    }
    this.greska= "";
    // @ts-ignore
    document.getElementById("email").style.border="1px solid rgb(128, 128, 128)";
    return false;
  }
}
