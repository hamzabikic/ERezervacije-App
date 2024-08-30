import {Grad} from "./KorisnikResponse";

export interface KorisnikManagerResponse {
  korisnikID: number;
  ime: string;
  prezime: string;
  username: string;
  email: string;
  brojTelefona: string;
  datumRodjenja:Date;
  gradID: number;
  grad: Grad;
}
export interface GostManagerResponse extends KorisnikManagerResponse{
  datumRegistracije: Date;
  brojRezervacija: number;
}
export interface HostesaManagerResponse extends  KorisnikManagerResponse {
  strucnaSprema:string;
  datumZaposlenja:Date;
}
export interface KorisnikManagerEdit {
  korisnikId:number;
  ime: string;
  prezime: string;
  gradId: number;
  datumRodjenja:string;
  brojTelefona: string;
  email: string;
  username: string;
}
export interface HostesaManagerEdit extends KorisnikManagerEdit{
  strucnaSprema:string;
  datumZaposlenja:string;
}
export interface KorisnikEditResponse {
  editovan:boolean;
  greska:string;
}
export interface HostesaAddManager {
  ime: string;
  prezime: string;
  username: string;
  email: string;
  brojTelefona: string;
  datumRodjenja: Date;
  gradID: number;
  strucnaSprema: string;
  datumZaposlenja: Date;
}
export interface AddHostesaResponse {
  hostesaId:number;
  greska:string;
}


