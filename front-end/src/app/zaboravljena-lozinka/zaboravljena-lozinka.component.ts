import { Component, OnInit } from '@angular/core';
import {LozinkaResponse} from "../KorisnikKlase/KorisnikResponse";
import {HttpClient} from "@angular/common/http";
import {Router} from "@angular/router";
import {MojConfig} from "../Servisi/MojConfig";
import {isNullOrEmpty} from "../Helpers/StringHelper";

@Component({
  selector: 'app-zaboravljena-lozinka',
  templateUrl: './zaboravljena-lozinka.component.html',
  styleUrls: ['./zaboravljena-lozinka.component.css']
})
export class ZaboravljenaLozinkaComponent implements OnInit {
  greska="";
  email ="";
  constructor(private http: HttpClient) { }

  ngOnInit(): void {
  }
  async zahtjevZaPromjenu() {
    if(isNullOrEmpty(this.email)) {
      this.greska ="Niste unijeli e-mail adresu!";
      return;
    }
    let res: LozinkaResponse = await this.http.get<LozinkaResponse>
    (MojConfig.adresa_servera +"ZaboravljenaLozinkaSlanje?email="+this.email).toPromise() as LozinkaResponse;
    if(res.promijenjena) {
      this.greska="";
      this.email="";
      alert("Provjerite svoje e-mail sanduče!");
      return;
    }
    else {
      this.greska=res.greska;
      return;
    }
  }

}
