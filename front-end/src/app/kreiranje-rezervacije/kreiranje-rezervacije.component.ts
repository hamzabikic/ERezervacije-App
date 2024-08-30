import { Component, OnInit } from '@angular/core';
import {RezervacijaPost, RezervacijaResponse, Stol, StoloviRequest, StoloviResponse} from "../RezervacijaKlase/Klase";
import {HttpClient} from "@angular/common/http";
import {MyAuth} from "../Servisi/MyAuth";
import {MojConfig} from "../Servisi/MojConfig";
import {DatePipe} from "@angular/common";
import {formatirajBroj} from "../Helpers/StringHelper";

@Component({
  selector: 'app-kreiranje-rezervacije',
  templateUrl: './kreiranje-rezervacije.component.html',
  styleUrls: ['./kreiranje-rezervacije.component.css']
})
export class KreiranjeRezervacijeComponent implements OnInit {

  datumRezervacije :string="";
  odLista: number[] = [];
  doLista: number[] = [];
  odSati: number = 0;
  doSati: number = 0;
  stolovi_pusacka: Stol[] = [];
  stolovi_nepusacka: Stol[] = [];
  stolovi_terasa: Stol[] = [];
  stolovi_odabrani: number[] = [];
  napomena = "";
  constructor(private http: HttpClient, private auth: MyAuth, private datePipe: DatePipe) {

  }

  async preuzmiStolove() {
    console.log(typeof(this.odSati));
    console.log(typeof(this.doSati));
    if(this.odSati >= this.doSati) {
      this.stolovi_nepusacka=[];
      this.stolovi_pusacka= [];
      this.stolovi_terasa= [];
      this.stolovi_odabrani= [];
      return;
    }

    let obj: StoloviRequest = {
      datumVrijeme: this.datumRezervacije + "T" + formatirajBroj(this.odSati) + ":00:00.000Z",
      trajanje: this.doSati - this.odSati
    };
    let res: StoloviResponse = await this.http.post(MojConfig.adresa_servera + "GetSlobodniStolovi", obj,
      {headers: {"my-token": this.auth.getToken()?.token!}}).toPromise() as StoloviResponse;
    this.stolovi_nepusacka = res.stolovi.filter((s) => !s.jePusackaZona && s.jeUnutra);
    this.stolovi_pusacka = res.stolovi.filter((s) => s.jePusackaZona && s.jeUnutra);
    this.stolovi_terasa = res.stolovi.filter((s) => s.jeUnutra == false);
    this.stolovi_odabrani= [];
  }

  async ngOnInit() {
    this.podesiVrijeme();
    this.popuniListu(this.odLista, 9, 22);
    this.popuniListu(this.doLista, 10, 23);
    await this.preuzmiStolove();
  }
   podesiVrijeme () {
    let currentTime = this.datePipe.transform(new Date(), 'yyyy-MM-dd HH:mm:ss', 'Europe/Sarajevo');
    let datum = new Date(currentTime!);
    this.odSati = ((datum.getHours()+7) >22 || (datum.getHours()+7) <9) ? 9 : datum.getHours()+7;
     this.doSati = ((datum.getHours()+7) >22 || (datum.getHours()+7) <9) ? 10 : datum.getHours()+8;
    if((datum.getHours()+7) >22) {
      datum = new Date(datum.setDate(datum.getDate()+1));
    }
     this.datumRezervacije = datum.getFullYear() +"-"+formatirajBroj(datum.getMonth()+1)+"-" +
       formatirajBroj(datum.getDate());
   }


  oznaci(stolId: number, event: Event) {
    if (this.stolovi_odabrani.includes(stolId)) {
      this.stolovi_odabrani = this.stolovi_odabrani.filter((s) => s != stolId);
      (event.target as HTMLElement).style.backgroundColor = "rgba(255, 255, 255, 0)";
      return;
    }
    if (this.stolovi_odabrani.length == 5) {
      alert("Maximalan broj stolova je 5!");
      return;
    }
    (event.target as HTMLElement).style.backgroundColor = "aqua";
    this.stolovi_odabrani.push(stolId);
  }

  popuniListu(lista: number[], odSati: number, doSati: number) {
    for (let i = odSati; i <= doSati; i++) {
      lista.push(i);
    }
  }

  async kreirajRezervaciju() {
    if (this.odSati >= this.doSati) {
      alert("Neispravno podešena satnica!");
      return;
    }
    if ((this.doSati - this.odSati) > 4) {
      alert("Maksimalno trajanje rezervacije je 4 sata!");
      return;
    }
    if (this.stolovi_odabrani.length == 0) {
      alert("Niste odabrali stolove!");
      return;
    }
    let obj: RezervacijaPost = {
      gostID: this.auth.getToken()?.gost?.korisnikID!,
      datumVrijeme: this.datumRezervacije + "T" + formatirajBroj(this.odSati) + ":00:00.000Z",
      trajanje: this.doSati - this.odSati,
      stolovi: this.stolovi_odabrani,
      posebneZelje: this.napomena
    };
    let res: RezervacijaResponse = await this.http.post(MojConfig.adresa_servera + "kreirajRezervaciju", obj,
      {headers: {"my-token": this.auth.getToken()?.token!}}).toPromise() as RezervacijaResponse;
    if (res.rezervacijaID != 0) {
      alert(`Uspjesno ste kreirali rezervaciju sa ID:${res.rezervacijaID}!`);
      this.stolovi_odabrani = [];
      let stolovi = Array.from(document.getElementsByClassName("stol"));
      for (let s of stolovi) {
        (s as HTMLElement).style.backgroundColor = "rgba(255, 255, 255, 0)";
      }
      this.napomena = "";
      await this.preuzmiStolove();
      return;
    } else {
      alert("Greska: " + res.greska);
    }
  }

  protected readonly formatirajBroj = formatirajBroj;
}
