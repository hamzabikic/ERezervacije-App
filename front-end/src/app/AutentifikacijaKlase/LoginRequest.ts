export interface PrijavaRequest{
  username:string,
  password:string
}
export interface Grad {
  gradID: number;
  naziv: string;
  ptt: number;
  drzava: string;
}

export interface Gost {
  korisnikID: number;
  ime: string;
  prezime: string;
  username: string;
  datumRegistracije: Date;
  brojRezervacija: number;
}
export interface Uposlenik {
  korisnikID: number;
  ime: string;
  prezime: string;
  username: string;
}
export interface TokenInformacije {
  token: string | null;
  isLogiran: boolean;
  isGost: boolean;
  isManager: boolean;
  isHostesa: boolean;
  gost?: Gost;
  uposlenik?: Uposlenik;
  geska:number;
}
export interface OdjavaResponse {
  odjavljen:boolean;
}
export interface Rezultat {
  postoji:boolean;
}
export interface RegistracijaRequest {
  ime: string;
  prezime: string;
  username: string;
  password: string;
  email: string;
  brojTelefona: string;
  datumRodjenja: Date;
  aktivan: boolean;
  gradID: number;
}
export interface RegistracijaResponse {
  registrovan:boolean;
  greska:string;
}
export interface PromjenaLozinkeRequest {
  key:string;
  novaLozinka:string;
}

