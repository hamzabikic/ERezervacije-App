import { Component, OnInit } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ActivatedRoute, Router} from "@angular/router";
import {MojConfig} from "../Servisi/MojConfig";
import {LozinkaResponse} from "../KorisnikKlase/KorisnikResponse";
import {isNullOrEmpty} from "../Helpers/StringHelper";
import {PromjenaLozinkeRequest} from "../AutentifikacijaKlase/LoginRequest";

@Component({
  selector: 'app-nova-lozinka',
  templateUrl: './nova-lozinka.component.html',
  styleUrls: ['./nova-lozinka.component.css']
})
export class NovaLozinkaComponent implements OnInit {
  lozinka="";
  ponovo_lozinka="";
  greska="";
  key="";
  omoguceno = false;
  constructor(private http:HttpClient, private router: Router, private route:ActivatedRoute) {
    this.key = route.snapshot.params["key"];
  }

  async ngOnInit() {
   await this.provjeraKljuca();
  }
  async provjeraKljuca() {
    let res = await this.http.get<boolean>(MojConfig.adresa_servera+"PostojiKljuc?key="+this.key).toPromise();
    if(!res) {
      this.router.navigate(["/login"]);
    }
    else {
      this.omoguceno = true;
    }
  }
  async promijeniLozinku() {
    if(isNullOrEmpty(this.lozinka) || isNullOrEmpty(this.ponovo_lozinka)){
      this.greska="Oba polja su obavezna za unos!";
      return;
    }
    if(this.lozinka.length<8 || this.ponovo_lozinka.length<8) {
      this.greska="Lozinka mora sadrzavati minimalno 8 karaktera!";
      return;
    }
    if(this.lozinka != this.ponovo_lozinka) {
      this.greska = "Unesene lozinke nisu podudarne!";
      this.lozinka="";
      this.ponovo_lozinka="";
      return;
    }
    let obj:PromjenaLozinkeRequest = {
      novaLozinka: this.lozinka,
      key: this.key
    };
    let res : LozinkaResponse = await this.http.post<LozinkaResponse>(MojConfig.adresa_servera+"PromjenaZaboravljeneLozinke", obj).toPromise() as LozinkaResponse;
    if(res.promijenjena) {
      this.greska="";
      alert("Uspjesno promijenjena lozinka!");
      this.router.navigate(["/login"]);
      return;
    }
    else {
      this.greska=res.greska;
    }
  }
}
