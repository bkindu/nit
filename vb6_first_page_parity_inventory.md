# VB6 First-Page Parity Inventory

## Birth and calendar data
The VB6 output prints Gender, father relationship, date of birth, birth time, English weekday, birthplace, longitude with direction, latitude with direction, Samvatsar, Vikrami Samvat, Saka Samvat, Ayan, Gola, Ritu, Saur Maas, and Paksha.

## Sunrise Panchang
The VB6 output prints sunrise Tithi, sunrise Nakshatra, sunrise Yoga, sunrise Karan, Sun Rise, Sun Set, Ishatkal in Ghatyadi, Dinman in Ghatyadi, and four ending-time lines for Tithi, Vaar, Karan, Yoga, and Nakshatra.

## Current Panchang
The VB6 output prints Tithi Bhukat/Bhogya, Hindu Vaar with the same Tithi Bhukat/Bhogya, Yoga Bhukat/Bhogya, Karan Bhukat/Bhogya, Nakshatra Bhukat/Bhogya, Charan, Nakshatra Bhukat Vikala, Bhabhog 48000 Vikalas, and Nakshatra Bhogya Vikala.

## Hora Chakram
The VB6 output prints Rashi, Rashipati, Varan, Vashya, Hansak, Rashi Name, Yoni, Gana, Nadi, Yunja, Name Letter, Varag, Paya, and Sun Sign.

## Ghat Chakram
The VB6 output prints Maas, Tithies, Vaar, Nakshatar, Yog, Karan, Prahar, and Lagan, followed by the nine planetary Ghat positions, Grah, Dhanya, Jati, Gender, Karuradi, Charadi, and Varna.

## Miscellaneous astronomy
The VB6 output prints Apparent Noon, G.M.T., Local Mean Time, Declination of Sun, Sidereal Time, Ayanamsa, Velantar, Chara Pal, Palbha in Anguladi, Mean Sun, War Time Correction, and True Rahu.

## Current gaps identified
The current `LegacyPanchangModel` only represents a subset of these fields. The current `LegacyPanchangCalculator` has placeholder Declination of Sun and does not expose the sunrise Panchang as a separate calculation. The current `Result.cshtml` has only three miscellaneous rows and simplified Hora/Ghat tables, so it cannot display all VB6 fields. The current calculator also uses approximated sunrise/sunset and sidereal calculations, and the complete legacy astronomy outputs must be added without deleting existing fields.

## VB6 authoritative mappings
`aPanchanga.bas` lines 60-178 calculate sunrise and birth-time Panchang independently. Lines 383-404 define the exact sunrise/current report mapping. Lines 410-470 define Hora, Ghat, and miscellaneous mappings. `a2Functions_Antronomy.bas` lines 1823-1926 define the FnPanchang formulas and `sData(0..38)` contract.

## Attachment reference case
Date: 08.02.1981; time: 07.47 AM; place: PATIALA; longitude: 76.25 EAST; latitude: 30.19 NORTH. The supplied VB6 output includes, among other values, sunrise 07:14:48, sunset 06:02:44, current Tithi CHATURTHI, Vaar RAVIVAAR, Yoga SIDDHA, Karan VANIJ, Nakshatra U.BHADRAPAD, Charan 2, Nakshatra Bhukat 13775 Vikala, and Nakshatra Bhogya 34225 Vikala.

This inventory is the parity checklist for implementation and validation.

