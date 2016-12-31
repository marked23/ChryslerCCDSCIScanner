- Gerber files are located in "gerbers" folder
- Gerbers are exported from EAGLE 7.4.0 using OSHPark's 2layer cam job
- Additional informations for each gerber file are in "infos" folder
- Rendered images of the PCB are in "renders" folder (ignore the gold finish)
- PCB board's exact size is 100x100 mm
- All holes are plated through holes (PTH), mounting holes included

- IMPORTANT: Please note that U1 (TQFP-100) and U2 (TQFP-32) chips have soldermask bridges 
  inbetween their pads. As long as they are manufacturable, please keep them. If not then 
  it's okay to remove them.

- Drills used:

 	Code  Size        Used     Size (mm)
	------------------------------------
 	T01   0.0157inch    86 --> 0.4 mm
 	T02   0.0197inch     5 --> 0.5 mm
 	T03   0.0315inch   185 --> 0.8 mm
 	T04   0.0394inch   142 --> 1.0 mm
 	T05   0.0472inch     5 --> 1.2 mm
 	T06   0.0551inch     6 --> 1.4 mm
 	T07   0.0630inch     3 --> 1.6 mm
 	T08   0.1378inch     4 --> 3.5 mm
 	T09   0.1575inch     4 --> 4.0 mm
	------------------------------------
	Total number of drills: 440
