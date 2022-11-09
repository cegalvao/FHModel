using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FHModel
{
    class FHData
    {
		public static List<(int, int)> Adj_04 = new() {
			(1, 27), (1, 25), (2, 18), (2, 3), (2, 17), (2, 16), 
			(2, 5), (3, 6), (3, 15), (3, 16), (3, 2), (4, 6), 
			(4, 21), (4, 15), (4, 8), (5, 15), (5, 16), (5, 17), 
			(5, 2), (6, 4), (6, 3), (6, 15), (8, 4), (8, 21), 
			(8, 23), (9, 12), (9, 10), (10, 9), (10, 12), (11, 14), 
			(11, 12), (11, 13), (12, 24), (12, 14), (12, 10), 
			(12, 9), (12, 11), (13, 11), (13, 31), (13, 14), 
			(14, 12), (14, 31), (14, 13), (14, 11), (14, 24), 
			(15, 3), (15, 6), (15, 16), (15, 5), (15, 4), 
			(16, 15), (16, 3), (16, 5), (16, 2), (17, 18), 
			(17, 2), (17, 20), (17, 19), (17, 5), (18, 20), 
			(18, 2), (18, 17), (19, 20), (19, 17), (20, 19), 
			(20, 17), (20, 18), (21, 4), (21, 8), (21, 23), 
			(23, 21), (23, 8), (24, 12), (24, 14), (24, 30), 
			(24, 31), (25, 1), (25, 26), (25, 27), (26, 25), 
			(26, 27), (27, 1), (27, 25), (27, 26), (28, 29), 
			(28, 32), (29, 28), (30, 32), (30, 24), (30, 31), 
			(31, 30), (31, 24), (31, 32), (31, 14), (31, 13), 
			(32, 30), (32, 31), (32, 28) };
        public static List<(string, double, double, bool, bool)> FMU_04 = new() {
			("F01", 10.16, 16.36, true, false), ("F02", 13.1, 15.5, true, false), 
			("F03", 10.44, 14.8, true, false), ("F04", 8.18, 10.42, true, false), 
			("F05", 13.48, 12.4, true, false), ("F06", 7.48, 12.18, true, false), 
			("F07", 0.62, 9.76, true, false), ("F08", 3.86, 9.34, true, false), 
			("F09", 6.0, 3.0, true, false), ("F10", 5.26, 4.8, true, false), 
			("F11", 10.14, 1.6, true, false), ("F12", 8.32, 3.78, true, false), 
			("F13", 11.66, 2.3, true, false), ("F14", 10.0, 5.0, true, false), 
			("F15", 10.2, 11.62, true, false), ("F16", 12.2, 12.96, true, false), 
			("F17", 15.74, 13.68, true, false), ("F18", 15.68, 16.8, true, false), 
			("F19", 18.58, 15.22, true, false), ("F20", 17.14, 16.9, true, false), 
			("F21", 6.48, 9.4, true, false), ("F22", 1.28, 6.64, true, false), 
			("F23", 4.74, 7.56, true, false), ("F24", 11.6, 6.58, true, false), 
			("F25", 5.66, 13.7, true, false), ("F26", 3.7, 11.88, true, false), 
			("F27", 4.42, 14.86, true, false), ("F28", 18.0, 5.0, true, false), 
			("F29", 18.48, 8.54, true, false), ("F30", 13.88, 8.18, true, false), 
			("F31", 12.88, 5.24, true, false), ("F32", 15.26, 4.26, true, false), 
			("C", 11.89, 16.32, false, false), ("D", 1.59, 8.98, false, false), 
			("E", 1, 14, false, false), ("G", 10.61, 8.14, false, false), 
			("H", 14, 11, false, false), //("I", 19.21, 12.26, false, false), 
			//("J", 16.41, 4.02, false, false), //("K", 16.583, 11.625, false, false), 
			("T01", 11.43, 15.99, false, false), ("T02", 10.3, 15.19, false, false), 
			("T03", 8.53, 13.93, false, false), ("T04", 6.84, 12.72, false, false), 
			("T05", 3.78, 10.54, false, false), ("T06", 1.43, 10.32, false, false), 
			("T07", 2.15, 7.56, false, false), ("T08", 16.44, 4.58, false, false), 
			("T09", 16.64, 8.4, false, false), ("T10", 18.53, 12.1, false, false), 
			("T11", 16.81, 11.68, false, false), ("T12", 13.64, 10.7, false, false), 
			("T13", 12.31, 9.58, false, false), ("T14", 10.5, 7.92, false, false), 
			("T15", 8.12, 7.15, false, false), ("T16", 4.99, 5.91, false, false), 
			("T17", 3.1, 5.16, false, false), ("T18", 2.4, 4.74, false, false), 
			("S", 0.83, 4.26, false, true) };
        public static List<(string, double, double)> AreaAge_04 = new() {
			("F01", 29.32071, 5), ("F02", 24.03103, 8), ("F03", 15.73957, 6), 
			("F04", 27.24917, 6), ("F05", 9.196134, 7), ("F06", 8.628516, 7), 
			("F07", 4.019982, 7), ("F08", 14.91321, 7), ("F09", 16.77808, 7), 
			("F10", 16.19167, 7), ("F11", 15.23158, 7), ("F12", 19.82241, 7), 
			("F13", 19.94975, 2), ("F14", 16.36772, 2), ("F15", 27.63576, 6), 
			("F16", 12.3976, 6), ("F17", 21.00163, 6), ("F18", 15.9561, 7), 
			("F19", 23.15135, 9), ("F20", 21.4856, 9), ("F21", 6.696061, 6), 
			("F22", 13.80046, 8), ("F23", 18.65147, 7), ("F24", 15.23001, 8), 
			("F25", 18.11244, 7), ("F26", 29.2073, 7), ("F27", 19.17439, 5), 
			("F28", 27.60491, 7), ("F29", 22.35679, 7), ("F30", 27.6433, 8), 
			("F31", 18.46308, 9), ("F32", 12.14387, 9) };
		public static List<(string, List<double>)> Vol_04 = new() {
		   ("F01", new List<double> { 0, 0, 0, 0, 392.0179, 444.6486}),
		   ("F02", new List<double> { 0, 319.084, 361.9313, 399.7802, 433.2795, 463.078}),
		   ("F03", new List<double> { 0, 0, 0, 145.0716, 164.5572, 181.7606}),
		   ("F04", new List<double> { 0, 0, 0, 364.3214, 413.2336, 456.4508}),
		   ("F05", new List<double> { 0, 0, 122.1063, 138.503, 152.9869, 165.8063}),
		   ("F06", new List<double> { 0, 0, 79.529, 90.2111, 99.6421, 107.9945}),
		   ("F07", new List<double> { 0, 0, 53.7472, 60.963, 67.3387, 72.9868}),
		   ("F08", new List<double> { 0, 0, 137.455, 155.9176, 172.2177, 186.6537}),
		   ("F09", new List<double> { 0, 0, 321.166, 364.3024, 402.3887, 436.1294}),
		   ("F10", new List<double> { 0, 0, 309.941, 351.5697, 388.3249, 420.8863}),
		   ("F11", new List<double> { 0, 0, 202.245, 229.4029, 253.3926, 274.6255}),
		   ("F12", new List<double> { 0, 0, 263.202, 298.5453, 329.7656, 357.3981}),
		   ("F13", new List<double> { 0, 0, 0, 0, 0, 0}),
		   ("F14", new List<double> { 0, 0, 0, 0, 0, 0}),
		   ("F15", new List<double> { 0, 0, 0, 369.4902, 419.0963, 462.9267}),
		   ("F16", new List<double> { 0, 0, 0, 165.7559, 188.0096, 207.6722}),
		   ("F17", new List<double> { 0, 0, 0, 194.9161, 221.0842, 244.207}),
		   ("F18", new List<double> { 0, 0, 148.0885, 167.9698, 185.5375, 201.0947}),
		   ("F19", new List<double> { 0, 348.6825, 385.1459, 417.4189, 446.1266, 471.7783}),
		   ("F20", new List<double> { 0, 323.5946, 357.4344, 387.3853, 414.0274, 437.8335}),
		   ("F21", new List<double> { 0, 0, 0, 89.5263, 101.5458, 112.1657}),
		   ("F22", new List<double> { 0, 183.2425, 207.8488, 229.5845, 248.8223, 265.9349}),
		   ("F23", new List<double> { 0, 0, 357.0265, 404.9794, 447.3183, 484.8264}),
		   ("F24", new List<double> { 0, 203.6252, 230.963, 255.1178, 276.516, 295.5383}),
		   ("F25", new List<double> { 0, 0, 166.9424, 189.3656, 209.1625, 226.6953}),
		   ("F26", new List<double> { 0, 0, 269.2037, 305.3623, 337.2859, 365.5585}),
		   ("F27", new List<double> { 0, 0, 0, 0, 256.3616, 290.7796}),
		   ("F28", new List<double> { 0, 0, 366.5379, 415.7575, 459.2352, 497.7165}),
		   ("F29", new List<double> { 0, 0, 296.8535, 336.7156, 371.9276, 403.0929}),
		   ("F30", new List<double> { 0, 256.5575, 291.0011, 321.4364, 348.3886, 372.3553}),
		   ("F31", new List<double> { 0, 272.3674, 300.8559, 326.0765, 348.4907, 368.5231}),
		   ("F32", new List<double> { 0, 179.1464, 197.8844, 214.473, 229.2156, 242.3917})};
        public static List<(string, List<double>)>  Prf_04 = new() {
			("F01", new List<double> {41397.2862, 0, 0, 0, 63016.3745, 47977.7753}),
			("F02", new List<double> {41757.275, 166334.582, 126644.6357, 94075.228, 68647.3942, 49436.5916}),
			("F03", new List<double> {16562.6778, 0, 0, 34181.2303, 26054.384, 19368.5213}),
			("F04", new List<double> {41913.04, 0, 0, 86689.3176, 66001.2677, 49027.318}),
			("F05", new List<double> {15068.0657, 0, 43001.3447, 32740.5737, 24320.6269, 17746.9425}),
			("F06", new List<double> {9745.9457, 0, 27737.3052, 21142.5509, 15717.1226, 11475.8756}),
			("F07", new List<double> {6633.6759, 0, 18930.8369, 14413.0704, 10706.3729, 7813.0068}),
			("F08", new List<double> {16844.5297, 0, 47940.1255, 36541.9977, 27164.8894, 19834.4761}),
			("F09", new List<double> {39826.8466, 0, 113861.9726, 86629.7902, 64314.9686, 46914.7022}),
			("F10", new List<double> {38434.8634, 0, 109882.3968, 83602.003, 62067.104, 45274.9922}),
			("F11", new List<double> {24957.2822, 0, 71223.2559, 54228.3101, 40282.327, 29394.3139}),
			("F12", new List<double> {32479.4524, 0, 92690.0747, 70572.8214, 52423.4937, 38253.8136}),
			("F13", new List<double> {26661.3111, 0, 0, 0, 0, 0}),
			("F14", new List<double> {21874.2079, 0, 0, 0, 0, 0}),
			("F15", new List<double> {42507.677, 0, 0, 87919.2133, 66937.6539, 49722.888}),
			("F16", new List<double> {19069.2437, 0, 0, 39441.1792, 30028.7037, 22306.0382}),
			("F17", new List<double> {22256.9728, 0, 0, 45935.2211, 35011.0208, 26027.3434}),
			("F18", new List<double> {18150.5608, 0, 51659.8172, 39374.2077, 29270.9552, 21372.4277}),
			("F19", new List<double> {42282.0315, 180602.6622, 134156.7807, 97895.2016, 70499.4728, 50261.8355}),
			("F20", new List<double> {39239.8093, 167608.1723, 124504.105, 90851.5723, 65426.9856, 46645.4607}),
			("F21", new List<double> {10299.4798, 0, 0, 21302.5557, 16218.7883, 12047.7033}),
			("F22", new List<double> {23980.2314, 95522.0803, 72729.0676, 54025.2146, 39422.6013, 28390.2843}),
			("F23", new List<double> {44273.7976, 0, 126575.4726, 96302.6231, 71496.1928, 52153.0627}),
			("F24", new List<double> {26652.0386, 106164.4687, 80828.754, 60041.5289, 43815.4807, 31554.1204}),
			("F25", new List<double> {20458.0781, 0, 58224.4117, 44381.117, 32992.3981, 24089.4385}),
			("F26", new List<double> {32989.7676, 0, 93890.0418, 71566.9736, 53202.0428, 38845.5344}),
			("F27", new List<double> {27071.9148, 0, 0, 0, 41209.8009, 31375.2511}),
			("F28", new List<double> {45231.2412, 0, 129081.2132, 98280.484, 73005.5314, 53272.6798}),
			("F29", new List<double> {36632.0893, 0, 104540.8971, 79595.8583, 59126.0615, 43144.7273}),
			("F30", new List<double> {33360.0551, 132479.7601, 100973.7524, 75064.3215, 54808.8292, 39488.7237}),
			("F31", new List<double> {33018.5839, 141017.8744, 104757.8252, 76447.0267, 55052.9345, 39249.5008}),
			("F32", new List<double> {21717.5844, 92752.8449, 68903.2249, 50282.1309, 36210.4189, 25815.8965})};
        public static List<(string, string, double, double, bool)>  Edges_04 = new() {
            ("F01", "F25", 5.227, 100, false), ("F01", "T01", 1.319, 10, false),
			("F01", "T02", 1.179, 10, false), ("F02", "F03", 3.015, 100, false),
			("F02", "F16", 2.695, 100, false), ("F02", "F18", 2.644, 100, false),
			("F02", "T01", 1.744, 10, false), ("F03", "F06", 3.517, 100, false),
			("F03", "F15", 2.496, 100, false), ("F03", "F16", 2.086, 100, false),
			("F03", "T02", 1.118, 10, false), ("F03", "T03", 1.912, 10, false),
			("F04", "F06", 1.894, 100, false), ("F04", "F15", 2.197, 100, false),
			("F04", "T14", 3.125, 10, false), ("F05", "F16", 1.397, 100, false),
			("F05", "F17", 2.597, 100, false), ("F05", "T12", 1.71, 10, false),
			("F06", "F15", 2.601, 100, false), ("F06", "F21", 3.295, 100, false),
			("F06", "T04", 0.84, 10, false), ("F07", "F22", 3.189, 100, false),
			("F07", "T06", 0.986, 10, false), ("F08", "F21", 2.637, 100, false),
			("F08", "F23", 1.986, 100, false), ("F08", "T05", 1.206, 10, false),
			("F08", "T07", 2.168, 10, false), ("F09", "F10", 1.309, 100, false),
			("F09", "F12", 2.448, 100, false), ("F10", "F12", 3.075, 100, false),
			("F10", "F14", 4.828, 100, false), ("F10", "T16", 1.849, 10, false),
			("F11", "F12", 2.84, 100, false), ("F11", "F13", 1.673, 100, false),
			("F12", "F14", 2.076, 100, false), ("F13", "F14", 3.169, 100, false),
			("F13", "F31", 3.183, 100, false), ("F14", "F24", 1.903, 100, false),
			("F14", "T15", 2.856, 10, false), ("F15", "F16", 2.559, 100, false),
			("F15", "T13", 3.071, 10, false), ("F17", "F18", 2.401, 100, false),
			("F17", "F19", 3.231, 100, false), ("F17", "T11", 2.268, 10, false),
			("F18", "F20", 1.675, 100, false), ("F19", "F20", 2.213, 100, false),
			("F19", "T10", 3.124, 10, false), ("F21", "T15", 2.502, 10, false),
			("F22", "T07", 1.567, 10, false), ("F22", "T18", 2.046, 100, false),
			("F23", "T16", 1.669, 10, false), ("F24", "F30", 3.242, 100, false),
			("F24", "F31", 2.26, 100, false), ("F24", "T14", 1.675, 10, false),
			("F25", "F26", 2.675, 100, false), ("F25", "F27", 1.698, 100, false),
			("F25", "T03", 2.883, 10, false), ("F25", "T04", 1.532, 10, false),
			("F26", "T05", 1.339, 10, false), ("F26", "T06", 2.753, 10, false),
			("F28", "F29", 3.572, 100, false), ("F28", "T08", 1.617, 10, false),
			("F29", "T09", 1.849, 10, false), ("F29", "T10", 3.557, 10, false),
			("F29", "T11", 3.557, 10, false), ("F30", "F31", 3.105, 100, false),
			("F30", "F32", 4.156, 100, false), ("F30", "T09", 2.765, 10, false),
			("F30", "T12", 2.529, 10, false), ("F30", "T13", 2.099, 10, false),
			("F31", "F32", 2.574, 100, false), ("F32", "T08", 1.221, 10, false),
			("C", "D", 12.648, 10, false), ("D", "E", 5.055, 10, false),
			("D", "T05", 2.694, 10, false), ("D", "T06", 1.349, 10, false),
			("D", "T07", 1.532, 10, false), ("G", "T13", 2.228, 10, false),
			("G", "T14", 0.597, 10, false), ("H", "T11", 2.888, 10, false),
			("H", "T12", 0.469, 10, false), ("T01", "T02", 1.378, 10, false),
			("T02", "T03", 2.173, 10, false), ("T03", "T04", 2.086, 10, false),
			("T04", "T05", 3.747, 10, false), ("T07", "T17", 2.575, 10, false),
			("T08", "T09", 3.822, 10, false), ("T09", "T11", 3.287, 10, false),
			("T10", "T11", 1.776, 10, false), ("T12", "T13", 1.738, 10, false),
			("T14", "T15", 2.078, 10, false), ("T15", "T16", 3.375, 10, false),
			("T16", "T17", 2.025, 10, false), ("T17", "T18", 1.151, 10, false),
			("T18", "S", 1.296, 10, false)
		};
        public static List<int> Fst_04 = new List<int> { 1 };//ToolBox.ReservoirSample(
                                                             //Enumerable.Range(0, AreaAge_04.Count).ToList(),
                                                             //Math.Max(1, (int)Math.Floor(AreaAge_04.Count / 10.0)));
        public static int Per_04 = Prf_04[0].Item2.Count;
        public static double Par_04_Cost_RoadOpening_ByUnity = 2.0;
        public static double Par_04_Cost_RoadMaintenance_ByUnity = 0.50;
        public static double Par_04_Cost_TravelFix_ByUnity = 0.30;
        public static double Par_04_Cost_TravelsIJ_ByUnity = 0.02;
        public static int Par_04_MaintenanceOpeningPeriods = 2;
        public static double Par_04_VolumeMin = 1.0;
        public static double Par_04_VolumeMax = 1000.0;
        public static double[] Par_04_Cost_RoadMaintenance_ByPeriod = { 2.0, 2.2, 2.35, 2.3, 2.8, 2.6, 3.05, 1.9, 2.6 };
        public static double[] Par_04_Cost_RoadOpening_ByPeriod = { 10.0, 11.1, 12.2, 13.3, 14.4, 15.3, 16.5, 17.0, 18.6 };
        public static double[] Par_04_Cost_TravelFix_ByPeriod = { 1.0, 1.1, 1.2, 1.3, 1.4, 1.3, 1.5, 1.0, 1.6 };
        public static double[] Par_04_BigM_S = { +0E0, +0E0, +0E0, +0E0, -2E4, +0E0, -2E4, +0E0, +0E0, -2E4, -2E4, +0E0, +0E0, -2E4, -2E4, +0E0, -2E4, -2E4, +0E0, +0E0, -2E4 };
        public static double[] Par_04_BigM_E = { -2E4, -2E4, -2E4, -2E4, +0E0, -2E4, -2E4, -2E4, -2E4, +0E0, +0E0, -2E4, -2E4, +0E0, -2E4, +0E0, +0E0, +0E0, -2E4, -2E4, +0E0 };
        public static double[] Par_04_SortimentDemand = { 0, 330, 275, 300, 400, 430}; //Média entre vol mín e vol max dos talhões por período
        public static int NumBlock_04 = 3;


        public static List<(int, int)> Adj_06 = new() {
            (1, 2), (1, 3), (1, 4), (2, 1), (2, 3), (2, 5),
            (3, 1), (3, 2), (3, 4), (3, 5), (4, 1), (4, 3),
            (4, 5), (4, 6), (5, 2), (5, 3), (5, 4), (5, 6),
            (6, 4), (6, 5) };
        public static List<(string, double, double, bool, bool)> FMU_06 = new() {
            ("S", 23.796, 4.546, false, true), ("1", 26.403, 19.148, true, false),
			("2", 27.001, 41.373, true, false), ("3", 45.24, 28.516, true, false),
			("4", 63.179, 26.523, true, false), ("5", 54.807, 53.632, true, false),
			("6", 83.61, 37.686, true, false) };
        public static List<(string, double, double)> AreaAge_06 = new() {
            ("1",21.1298184,2), ("2",20.10375183,3), ("3",19.589836,2),
			("4",19.287054,2), ("5",13.915089,3), ("6",16.8010132,3) };
        public static List<(string, List<double>)> Vol_06 = new() {
            ("1", new List<double> { 0.0, 105.67793, 182.30734, 259.77413 }),
			("2", new List<double> { 0.0, 160.893125, 302.3021611, 372.9390583 }),
			("3", new List<double> { 0.0, 104.20401, 224.0913133, 282.3369 }),
			("4", new List<double> { 0.0, 93.36867, 215.3792667, 274.47773 }),
			("5", new List<double> { 0.0, 100.9495333, 191.0782778, 257.2082333 }),
			("6", new List<double> { 0.0, 133.87909, 225.7872333, 256.91512 })};
        public static List<(string, List<double>)> Prf_06 = new() {
            ("1", new List<double> {29992.81446, 42758.03216, 41885.14453, 34232.2415}),
			("2", new List<double> {35208.18672, 64809.87728, 70773.44377, 49244.48017}),
			("3", new List<double> {26659.6066, 40796.91106, 51870.00403, 37140.74434}),
			("4", new List<double> {30405.7316, 36275.73757, 49758.20392, 36234.56165}),
			("5", new List<double> {24455.64777, 40564.21125, 44284.53437, 34004.68626}),
			("6", new List<double> {24014.76926, 52242.38861, 54464.59589, 33766.79713})};
        public static List<(string, string, double, double, bool)> Edges_06 = new() {
            ("S", "1", 14.83290, 10, false), ("1", "2", 22.23304, 10, false),
			("1", "3", 21.03787, 10, false), ("1", "4", 37.50820, 10, false),
			("2", "5", 30.38843, 10, false), ("3", "4", 18.04937, 10, false),
			("3", "5", 26.87640, 10, false), ("4", "6", 23.28172, 10, false),
			("5", "6", 32.92245, 10, false) };
        public static List<int> Fst_06 = new List<int> { 1 };//ToolBox.ReservoirSample(
                                                             //Enumerable.Range(0, AreaAge_04.Count).ToList(),
                                                             //Math.Max(1, (int)Math.Floor(AreaAge_04.Count / 10.0)));
        public static int Per_06 = Prf_06[0].Item2.Count;
        public static double Par_06_Cost_RoadOpening_ByUnity = 1.0;
        public static double Par_06_Cost_RoadMaintenance_ByUnity = 0.250;
        public static double Par_06_Cost_TravelFix_ByUnity = 0.30;
        public static double Par_06_Cost_TravelsIJ_ByUnity = 0.002;
        public static int Par_06_MaintenanceOpeningPeriods = 2;
        public static double Par_06_VolumeMin = 1.0;
        public static double Par_06_VolumeMax = 400.0;
        public static double[] Par_06_Cost_RoadMaintenance_ByPeriod = { 0.20, 0.22, 0.235, 0.23 };
        public static double[] Par_06_Cost_RoadOpening_ByPeriod = { 1.0, 1.11, 1.22, 1.33, };
        public static double[] Par_06_Cost_TravelFix_ByPeriod = { 1.4, 1.3, 1.5, 1.0};
        public static double[] Par_06_BigM_S = { +0E0, +0E0, +0E0, +0E0, -2E4, +0E0, -2E4, +0E0, +0E0, -2E4, -2E4, +0E0, +0E0, -2E4, -2E4, +0E0, -2E4, -2E4, +0E0, +0E0, -2E4 };
        public static double[] Par_06_BigM_E = { -2E4, -2E4, -2E4, -2E4, +0E0, -2E4, -2E4, -2E4, -2E4, +0E0, +0E0, -2E4, -2E4, +0E0, -2E4, +0E0, +0E0, +0E0, -2E4, -2E4, +0E0 };
        public static double[] Par_06_SortimentDemand = { 0.0, 127.0, 242.0, 315.0 }; //Média entre vol mín e vol max dos talhões por período
        public static int NumBlock_06 = 3;

        public static List<(int, int)> Adj_10 = new() {
			(0, 1), (1, 0), (3, 4), (4, 3), (4, 5), (5, 4), (5, 6), 
			(6, 5), (7, 8), (8, 7), (8, 9), (9, 8) };
        public static List<(string, double, double, bool, bool)>  FMU_10 = new()  {
			("F01", 10.16, 16.36, true, false), ("F02", 5.48, 13.068, true, false),
			("F03", 0.95, 8.2, true, false), ("F04", 5.027, 8.647, true, false),
			("F05", 8.56, 11.407, true, false), ("F06", 12.325, 13.952, true, false),
			("F07", 16.828, 15.363, true, false), ("F08", 18.24, 6.77, true, false),
			("F09", 13.368, 6.246, true, false), ("F10", 8.733, 3.343, true, false),
			("C", 11.89, 16.32, false, false), ("D", 1.59, 8.98, false, false),
			("E", 1.0, 14.0, false, false), ("F", 3.105, 5.162, false, false),
			("G", 10.61, 8.14, false, false), ("H", 14.0, 11.0, false, false),
			("I", 19.21, 12.26, false, false), ("J", 16.41, 4.02, false, false),
			("K", 16.583, 11.625, false, false), ("T01", 10.858, 15.584, false, false),
			("T02", 6.531, 12.501, false, false), ("T03", 5.334, 11.648, false, false),
			("T04", 1.86, 8.3, false, false), ("T05", 6.516, 6.516, false, false),
			("T06", 12.856, 10.034, false, false), ("T07", 17.409, 11.825, false, false),
			("T08", 16.468, 6.579, false, false), ("S", 0.83, 4.26, false, true)};
        public static List<(string, double, double)>  AreaAge_10 = new() {
			("F01", 29.32071, 1.0), ("F02", 66.49413, 1.33333333333333),
			("F03", 17.820442, 2.5), ("F04", 40.260741, 1.66666666666667),
			("F05", 63.513446, 1.33333333333333), ("F06", 61.364334, 1.75),
			("F07", 81.59468, 2.75), ("F08", 49.9617, 2.0),
			("F09", 73.48026, 3.5), ("F10", 104.34121, 0.33333333333333)};
        public static List<(string, List<double>)>  Vol_10 = new() {
			("F01", new List<double> { 0.0, 0.0, 0.0, 0.0, 392.0179, 444.6486}),
			("F02", new List<double> { 0.0, 0.0, 436.1461, 494.7279, 802.81, 883.0334}),
			("F03", new List<double> { 0.0, 183.2425, 261.596, 290.5475, 316.161, 338.9217}),
			("F04", new List<double> { 0.0, 0.0, 494.4815, 650.4233, 721.0818, 783.6458}),
			("F05", new List<double> { 0.0, 0.0, 79.529, 824.0227, 931.972, 1027.372}),
			("F06", new List<double> { 0.0, 319.084, 484.0376, 849.1107, 938.8332, 1018.3171}),
			("F07", new List<double> { 0.0, 672.2771, 890.6688, 1167.6901, 1266.7757, 1354.9135}),
			("F08", new List<double> { 0.0, 0.0, 663.3914, 752.4731, 831.1628, 900.8094}),
			("F09", new List<double> { 0.0, 911.6965, 1020.7044, 1117.1037, 1202.6109, 1278.8084}),
			("F10", new List<double> { 0.0, 0.0, 1096.554, 1243.8203, 1373.8718, 1489.0393})};
        public static List<(string, List<double>)>  Prf_10 = new() {
			("F01", new List<double> {41397.2862, 0.0, 0.0, 0.0, 63016.3745, 47977.7753}),
			("F02", new List<double> {80519.7605, 0.0, 152114.4535, 115948.0906, 127404.2418, 94310.224}),
			("F03", new List<double> {30613.9073, 95522.0803, 91659.9045, 68438.285, 50128.9742, 36203.2911}),
			("F04", new List<double> {71417.8071, 0.0, 174515.5981, 154147.1765, 114879.8705, 84035.2421}),
			("F05", new List<double> {94166.6627, 0.0, 27737.3052, 195751.0818, 148656.0442, 110226.0816}),
			("F06", new List<double> {92457.2622, 166334.582, 169645.9804, 200438.2112, 149051.1088, 108858.0936}),
			("F07", new List<double> {121929.3744, 348210.8345, 310320.7029, 274056.2027, 200208.4344, 144307.0673}),
			("F08", new List<double> {81863.3305, 0.0, 233622.1103, 177876.3423, 132131.5929, 96417.4071}),
			("F09", new List<double> {114748.262, 472414.9481, 355463.5565, 261835.008, 189887.6633, 136108.2414}),
			("F10", new List<double> {184233.9636, 0.0, 387657.7, 295032.9247, 219087.8933, 159837.8219})};
        public static List<(string, string, double, double, bool)>  Edges_10 = new() {
			("F01", "F02", 5.722, 100, false), ("F01", "T01", 1.43, 100, false), 
			("F02", "F01", 5.722, 100, false), ("F02", "D", 5.643, 100, false), 
			("F02", "T02", 1.195, 100, false), ("F02", "T03", 1.428, 100, false), 
			("F03", "D", 1.9, 100, false), ("F03", "T04", 0.915, 100, false), 
			("F04", "F05", 4.484, 100, false), ("F04", "T03", 3.17, 100, false), 
			("F04", "T04", 3.186, 100, false), ("F04", "T05", 2.6, 100, false), 
			("F05", "F04", 4.484, 100, false), ("F05", "F06", 4.545, 100, false), 
			("F05", "T02", 2.305, 100, false), ("F06", "F05", 4.545, 100, false), 
			("F06", "F07", 4.718, 100, false), ("F06", "T01", 2.195, 100, false), 
			("F06", "T06", 3.954, 100, false), ("F07", "F06", 4.718, 100, false), 
			("F07", "T07", 3.586, 100, false), ("F08", "T07", 5.123, 100, false), 
			("F08", "T08", 1.782, 100, false), ("F09", "F10", 5.469, 100, false), 
			("F09", "T06", 3.823, 100, false), ("F09", "T08", 3.118, 100, false), 
			("F10", "F09", 5.469, 100, false), ("F10", "T05", 3.871, 100, false), 
			("C", "T01", 1.268, 10, false), ("D", "E", 5.055, 10, false), 
			("D", "F02", 5.643, 100, false), ("D", "F03", 1.9, 100, false), 
			("D", "T03", 4.597, 10, false), ("D", "T04", 0.732, 10, false), 
			("E", "D", 5.055, 10, false), ("F", "T04", 3.376, 10, false), 
			("F", "T05", 3.67, 10, false), ("F", "S", 2.447, 10, false), 
			("G", "T05", 4.404, 10, false), ("G", "T06", 2.938, 10, false), 
			("I", "T07", 1.853, 10, false), ("J", "T08", 2.56, 10, false), 
			("H", "T06", 1.497, 10, false), 
			("K", "H", 2.658, 10, false), ("K", "T07", 0.85, 10, false), 
			("K", "T08", 5.47, 10, false), ("H", "K", 2.658, 10, false),
			("T01", "F06", 2.195, 100, false), ("T01", "C", 1.268, 10, false), 
			("T01", "T02", 5.313, 10, false), ("T02", "F02", 1.195, 100, false), 
			("T02", "F05", 2.305, 100, false), ("T02", "T01", 5.313, 10, false), 
			("T02", "T03", 1.47, 10, false), ("T03", "F02", 1.428, 100, false), 
			("T03", "F04", 3.17, 100, false), ("T03", "D", 4.597, 10, false), 
			("T03", "T02", 1.47, 10, false), ("T04", "F03", 0.915, 100, false), 
			("T04", "F04", 3.186, 100, false), ("T04", "D", 0.732, 10, false), 
			("T04", "F", 3.376, 10, false), ("T05", "F04", 2.6, 100, false), 
			("T05", "F10", 3.871, 100, false), ("T05", "F", 3.67, 10, false), 
			("T05", "G", 4.404, 10, false), ("T06", "F06", 3.954, 100, false), 
			("T06", "F09", 3.823, 100, false), ("T06", "G", 2.938, 10, false), 
			("T06", "H", 1.497, 10, false), ("T07", "F07", 3.586, 100, false), 
			("T07", "F08", 5.123, 100, false), ("T07", "I", 1.853, 10, false), 
			("T07", "K", 0.85, 10, false), ("T08", "K", 5.47, 10, false), 
			("T08", "F09", 3.118, 100, false), ("T08", "J", 2.56, 10, false), 
			 ("T08", "F08", 1.782, 100, false)};
        public static List<int> Fst_10 = new List<int> { 1 };//ToolBox.ReservoirSample(
                                                             //Enumerable.Range(0, AreaAge_10.Count).ToList(),
                                                             //Math.Max(1, (int)Math.Floor(AreaAge_10.Count / 10.0)));
        public static int Per_10 = Prf_10[0].Item2.Count;
        public static double Par_10_Cost_RoadOpening_ByUnity = 2.0;
        public static double Par_10_Cost_RoadMaintenance_ByUnity = 0.50;
        public static double Par_10_Cost_TravelFix_ByUnity = 0.30;
        public static double Par_10_Cost_TravelsIJ_ByUnity = 0.02;
        public static int Par_10_MaintenanceOpeningPeriods = 3;
        public static double Par_10_VolumeMin = 1.0;
        public static double Par_10_VolumeMax = 1000.0;
        public static double[] Par_10_Cost_RoadOpening_ByPeriod = { 10.0, 11.1, 12.2, 13.3, 14.4, 15.3, 17.0, 18.6 };
        public static double[] Par_10_Cost_RoadMaintenance_ByPeriod = { 2.0, 2.2, 2.35, 2.3, 2.8, 2.6, 3.05, 1.9, 2.6 };
        public static double[] Par_10_Cost_TravelFix_ByPeriod = { 1.0, 1.1, 1.2, 1.3, 1.4, 1.3, 1.5, 1.0, 1.6 };
        public static double[] Par_10_BigM_S = { +0E0, +0E0, +0E0, +0E0, -2E4, +0E0, -2E4, +0E0, +0E0, -2E4, -2E4, +0E0, +0E0, -2E4, -2E4, +0E0, -2E4, -2E4, +0E0, +0E0, -2E4 };
        public static double[] Par_10_BigM_E = { -2E4, -2E4, -2E4, -2E4, +0E0, -2E4, -2E4, -2E4, -2E4, +0E0, +0E0, -2E4, -2E4, +0E0, -2E4, +0E0, +0E0, +0E0, -2E4, -2E4, +0E0 };
        public static double[] Par_10_SortimentDemand = { 0, 455.84825, 548.277, 621.91015, 845.0164, 913.9805 }; //Média entre vol mín e vol max dos talhões por período
        public static int NumBlock_10 = 3;

        public static List<(int, int)> Adj_21 = new() {
			 (0, 1), (0, 7), (1, 2), (1, 8), (2, 3), (2, 9), 
			(3, 4), (3, 10), (4, 5), (4, 11), (5, 6), (5, 12), 
			(6, 13), (7, 8), (7, 14), (8, 9), (8, 15), (9, 10), 
			(9, 16), (10, 11), (10, 17), (11, 12), (11, 18), 
			(12, 13), (12, 19), (13, 20), (14, 15), (15, 16), 
			(16, 17), (17, 18), (18, 19), (19, 20) };
        public static List<(string, double, double, bool, bool)>  FMU_21 = new()  {
			("11", 1.0, 1.0, true, false), ("12", 1.0, 2.0, true, false),
			("13", 1.0, 3.0, true, false), ("14", 1.0, 4.0, true, false),
			("15", 1.0, 5.0, true, false), ("16", 1.0, 6.0, true, false),
			("17", 1.0, 7.0, true, false), ("21", 2.0, 1.0, true, false),
			("22", 2.0, 2.0, true, false), ("23", 2.0, 3.0, true, false),
			("24", 2.0, 4.0, true, false), ("25", 2.0, 5.0, true, false),
			("26", 2.0, 6.0, true, false), ("27", 2.0, 7.0, true, false),
			("31", 3.0, 1.0, true, false), ("32", 3.0, 2.0, true, false),
			("33", 3.0, 3.0, true, false), ("34", 3.0, 4.0, true, false),
			("35", 3.0, 5.0, true, false), ("36", 3.0, 6.0, true, false),
			("37", 3.0, 7.0, true, true)};
        public static List<(string, double, double)>  AreaAge_21 = new() {
			("11", 29.32071, 5.0), ("12", 24.03103, 8.0), ("13", 15.73957, 6.0),
			("14", 27.24917, 6.0), ("15", 9.196134, 7.0), ("16", 8.628516, 7.0),
			("17", 4.019982, 7.0), ("21", 14.91321, 7.0), ("22", 16.77808, 7.0),
			("23", 16.19167, 7.0), ("24", 15.23158, 7.0), ("25", 19.82241, 7.0),
			("26", 19.94975, 2.0), ("27", 16.36772, 2.0), ("31", 27.63576, 6.0),
			("32", 12.3976, 6.0), ("33", 21.00163, 6.0), ("34", 15.9561, 7.0),
			("35", 23.15135, 9.0), ("36", 21.4856, 9.0), ("37", 6.696061, 6.0)};
        public static List<(string, List<double>)>  Vol_21 = new() {
			("11", new List<double> { 0.0, 0.0, 0.0, 0.0, 392.0179, 444.6486}),
			("12", new List<double> { 0.0, 319.084, 361.9313, 399.7802, 433.2795, 463.078}),
			("13", new List<double> { 0.0, 0.0, 0.0, 145.0716, 164.5572, 181.7606}),
			("14", new List<double> { 0.0, 0.0, 0.0, 364.3214, 413.2336, 456.4508}),
			("15", new List<double> { 0.0, 0.0, 122.1063, 138.503, 152.9869, 165.8063}),
			("16", new List<double> { 0.0, 0.0, 79.529, 90.2111, 99.6421, 107.9945}),
			("17", new List<double> { 0.0, 0.0, 53.7472, 60.963, 67.3387, 72.9868}),
			("21", new List<double> { 0.0, 0.0, 137.455, 155.9176, 172.2177, 186.6537}),
			("22", new List<double> { 0.0, 0.0, 321.166, 364.3024, 402.3887, 436.1294}),
			("23", new List<double> { 0.0, 0.0, 309.941, 351.5697, 388.3249, 420.8863}),
			("24", new List<double> { 0.0, 0.0, 202.245, 229.4029, 253.3926, 274.6255}),
			("25", new List<double> { 0.0, 0.0, 263.202, 298.5453, 329.7656, 357.3981}),
			("26", new List<double> { 0.0, 0.0, 0.0, 369.4902, 419.0963, 462.9267}),
			("27", new List<double> { 0.0, 0.0, 0.0, 165.7559, 188.0096, 207.6722}),
			("31", new List<double> { 0.0, 183.2425, 207.8488, 229.5845, 248.8223, 265.9349}),
			("32", new List<double> { 0.0, 0.0, 148.0885, 167.9698, 185.5375, 201.0947}),
			("33", new List<double> { 0.0, 348.6825, 385.1459, 417.4189, 446.1266, 471.7783}),
			("34", new List<double> { 0.0, 323.5946, 357.4344, 387.3853, 414.0274, 437.8335}),
			("35", new List<double> { 0.0, 0.0, 0.0, 89.5263, 101.5458, 112.1657}),
			("36", new List<double> { 0.0, 0.0, 357.0265, 404.9794, 447.3183, 484.8264}),
			("37", new List<double> { 0.0, 203.6252, 230.963, 255.1178, 276.516, 295.5383})};
        public static List<(string, List<double>)>  Prf_21 = new() {
			("11", new List<double> {41397.2862, 0.0, 0.0, 0.0, 63016.3745, 47977.7753}),
			("12", new List<double> {41757.275, 166334.582, 126644.6357, 94075.228, 68647.3942, 49436.5916}),
			("13", new List<double> {16562.6778, 0.0, 0.0, 34181.2303, 26054.384, 19368.5213}),
			("14", new List<double> {41913.04, 0.0, 0.0, 86689.3176, 66001.2677, 49027.318}),
			("15", new List<double> {15068.0657, 0.0, 43001.3447, 32740.5737, 24320.6269, 17746.9425}),
			("16", new List<double> {9745.9457, 0.0, 27737.3052, 21142.5509, 15717.1226, 11475.8756}),
			("17", new List<double> {6633.6759, 0.0, 18930.8369, 14413.0704, 10706.3729, 7813.0068}),
			("21", new List<double> {16844.5297, 0.0, 47940.1255, 36541.9977, 27164.8894, 19834.4761}),
			("22", new List<double> {39826.8466, 0.0, 113861.9726, 86629.7902, 64314.9686, 46914.7022}),
			("23", new List<double> {38434.8634, 0.0, 109882.3968, 83602.003, 62067.104, 45274.9922}),
			("24", new List<double> {24957.2822, 0.0, 71223.2559, 54228.3101, 40282.327, 29394.3139}),
			("25", new List<double> {32479.4524, 0.0, 92690.0747, 70572.8214, 52423.4937, 38253.8136}),
			("26", new List<double> {42507.677, 0.0, 0.0, 87919.2133, 66937.6539, 49722.888}),
			("27", new List<double> {19069.2437, 0.0, 0.0, 39441.1792, 30028.7037, 22306.0382}),
			("31", new List<double> {21717.5844, 92752.8449, 68903.2249, 50282.1309, 36210.4189, 25815.8965}),
			("32", new List<double> {18150.5608, 0.0, 51659.8172, 39374.2077, 29270.9552, 21372.4277}),
			("33", new List<double> {42282.0315, 180602.6622, 134156.7807, 97895.2016, 70499.4728, 50261.8355}),
			("34", new List<double> {39239.8093, 167608.1723, 124504.105, 90851.5723, 65426.9856, 46645.4607}),
			("35", new List<double> {10299.4798, 0.0, 0.0, 21302.5557, 16218.7883, 12047.7033}),
			("36", new List<double> {44273.7976, 0.0, 126575.4726, 96302.6231, 71496.1928, 52153.0627}),
			("37", new List<double> {26652.0386, 106164.4687, 80828.754, 60041.5289, 43815.4807, 31554.1204})};
        public static List<(string, string, double, double, bool)>  Edges_21 = new() {
		 	("11", "12", 1.0, 10, false), ("11", "21", 1.0, 10, false), 
			("12", "13", 1.0, 10, false), ("12", "22", 1.0, 10, false), 
			("13", "14", 1.0, 10, false), ("13", "23", 1.0, 10, false), 
			("14", "15", 1.0, 10, false), ("14", "24", 1.0, 10, false), 
			("15", "16", 1.0, 10, false), ("15", "25", 1.0, 10, false), 
			("16", "17", 1.0, 10, false), ("16", "26", 1.0, 10, false), 
			("17", "27", 1.0, 10, false), ("21", "22", 1.0, 10, false), 
			("21", "31", 1.0, 10, false), ("22", "23", 1.0, 10, false), 
			("22", "32", 1.0, 10, false), ("23", "24", 1.0, 10, false), 
			("23", "33", 1.0, 10, false), ("24", "25", 1.0, 10, false), 
			("24", "34", 1.0, 10, false), ("25", "26", 1.0, 10, false), 
			("25", "35", 1.0, 10, false), ("26", "27", 1.0, 10, false), 
			("26", "36", 1.0, 10, false), ("27", "37", 1.0, 10, false), 
			("31", "32", 1.0, 10, false), ("32", "33", 1.0, 10, false), 
			("33", "34", 1.0, 10, false), ("34", "35", 1.0, 10, false), 
			("35", "36", 1.0, 10, false), ("36", "37", 1.0, 10, false)};


        
        public static List<int> Fst_21 = new List<int> { 1 };//ToolBox.ReservoirSample(
                                                             //Enumerable.Range(0, AreaAge_21.Count).ToList(),
                                                             //Math.Max(1, (int)Math.Floor(AreaAge_21.Count / 10.0)));
        public static int Per_21 = Prf_21[0].Item2.Count;
        public static double Par_21_Cost_RoadOpening_ByUnity = 2.0;
        public static double Par_21_Cost_RoadMaintenance_ByUnity = 0.50;
        public static double Par_21_Cost_TravelFix_ByUnity = 0.30;
        public static double Par_21_Cost_TravelsIJ_ByUnity = 0.02;
        public static int Par_21_MaintenanceOpeningPeriods = 3;
        public static double Par_21_VolumeMin = 10.0;
        public static double Par_21_VolumeMax = 1500.0;
        public static double[] Par_21_Cost_RoadOpening_ByPeriod = { 10.0, 11.1, 12.2, 13.3, 14.4, 15.3, 16.5, 17.0, 18.6 };
        public static double[] Par_21_Cost_RoadMaintenance_ByPeriod = { 2.0, 2.2, 2.35, 2.3, 2.8, 2.6, 3.05, 1.9, 2.6 };
        public static double[] Par_21_Cost_TravelFix_ByPeriod = { 1.0, 1.1, 1.2, 1.3, 1.4, 1.3, 1.5, 1.0, 1.6 };
		public static double[] Par_21_BigM_S = {+0E0, +0E0, +0E0, +0E0, -2E4, +0E0, -2E4, +0E0, +0E0, -2E4, -2E4, +0E0, +0E0, -2E4, -2E4, +0E0, -2E4, -2E4, +0E0, +0E0, -2E4 };
        public static double[] Par_21_BigM_E = {-2E4, -2E4, -2E4, -2E4, +0E0, -2E4, -2E4, -2E4, -2E4, +0E0, +0E0, -2E4, -2E4, +0E0, -2E4, +0E0, +0E0, +0E0, -2E4, -2E4, +0E0 };
		public static double[] Par_21_SortimentDemand = { 0, 174.34125, 192.57295, 208.70945, 257.3285, 278.9066 };



		public static List<(int, int, int)> Par_04_SolTest = new() { (0, 3, 1),
			(1, 4, 1), (2, 2, 1), (3, 2, 1), (4, 1, 1), (5, 1, 1), (6, 2, 1), 
			(7, 1, 1), (8, 1, 1), (9, 2, 1), (10, 1, 1), (11, 2, 1), (12, 0, 1),
			(13, 0, 1), (14, 2, 1), (15, 2, 1), (16, 2, 1), (17, 1, 1), (18, 2, 1), 
			(19, 1, 1), (20, 3, 1), (21, 1, 1), (22, 1, 1), (23, 1, 1), (24, 1, 1),
			(25, 1, 1), (26, 4, 1), (27, 1, 1), (28, 1, 1), (29, 4, 1), (30, 2, 1),
			(31, 1, 1)};
        public static List<(int, int, int)> Par_10_SolTest = new() { (0, 3, 1), 
			(1, 1, 1), (2, 1, 1), (3, 1, 1), (4, 1, 1), (5, 2, 1), (6, 1, 1), 
			(7, 1, 1), (8, 1, 1), (9, 1, 1) };
        public static List<(int, int, int)> Par_06_SolTest = new() { };
        public static List<(int, int, int)> Par_21_SolTest = new() { (0, 4, 1),
			(1, 1, 1), (2, 2, 1), (3, 4, 1), (4, 3, 1), (5, 1, 1), (6, 1, 1), 
			(7, 1, 1), (8, 3, 1), (9, 1, 1), (10, 1, 1), (11, 1, 1), (12, 2, 1),
			(13, 2, 1), (14, 1, 1), (15, 1, 1), (16, 4, 1), (17, 1, 1), (18, 4, 1), 
			(19, 1, 1), (20, 2, 1) };
        public static int NumBlock_21 = 3;

        public static DataMatrixPar Par_04 = new(Par_04_Cost_RoadOpening_ByUnity,
			                                     Par_04_Cost_RoadMaintenance_ByUnity, 
												 Par_04_Cost_TravelFix_ByUnity, 
												 Par_04_Cost_TravelsIJ_ByUnity, 
												 Par_04_MaintenanceOpeningPeriods, 
												 Par_04_VolumeMin, Par_04_VolumeMax,
                                                 AreaAge_04, Par_04_Cost_RoadOpening_ByPeriod, 
												 Par_04_Cost_RoadMaintenance_ByPeriod, 
												 Par_04_Cost_TravelFix_ByPeriod, Par_04_BigM_S, 
												 Par_04_BigM_E, Par_04_SortimentDemand,
												 Fst_04, Per_04, NumBlock_04, Par_04_SolTest);

        public static DataMatrixPar Par_06 = new(Par_06_Cost_RoadOpening_ByUnity,
                                                 Par_06_Cost_RoadMaintenance_ByUnity,
                                                 Par_06_Cost_TravelFix_ByUnity,
                                                 Par_06_Cost_TravelsIJ_ByUnity,
                                                 Par_06_MaintenanceOpeningPeriods,
                                                 Par_06_VolumeMin, Par_06_VolumeMax,
                                                 AreaAge_06, Par_06_Cost_RoadOpening_ByPeriod,
                                                 Par_06_Cost_RoadMaintenance_ByPeriod,
                                                 Par_06_Cost_TravelFix_ByPeriod, Par_06_BigM_S,
                                                 Par_06_BigM_E, Par_06_SortimentDemand,
                                                 Fst_06, Per_06, NumBlock_06, Par_06_SolTest);

        public static DataMatrixPar Par_10 = new(Par_10_Cost_RoadOpening_ByUnity, 
			                                     Par_10_Cost_RoadMaintenance_ByUnity, 
												 Par_10_Cost_TravelFix_ByUnity, 
												 Par_10_Cost_TravelsIJ_ByUnity, 
												 Par_10_MaintenanceOpeningPeriods, 
												 Par_10_VolumeMin, Par_10_VolumeMax, 
												 AreaAge_10, Par_10_Cost_RoadOpening_ByPeriod, 
												 Par_10_Cost_RoadMaintenance_ByPeriod, 
												 Par_10_Cost_TravelFix_ByPeriod, Par_10_BigM_S, 
												 Par_10_BigM_E, Par_10_SortimentDemand,
                                                 Fst_10, Per_10, NumBlock_10, Par_10_SolTest);
		public static DataMatrixPar Par_21 = new(Par_21_Cost_RoadOpening_ByUnity, 
			                                     Par_21_Cost_RoadMaintenance_ByUnity, 
												 Par_21_Cost_TravelFix_ByUnity, 
												 Par_21_Cost_TravelsIJ_ByUnity, 
												 Par_21_MaintenanceOpeningPeriods, 
												 Par_21_VolumeMin, Par_21_VolumeMax, 
												 AreaAge_21, Par_21_Cost_RoadOpening_ByPeriod, 
												 Par_21_Cost_RoadMaintenance_ByPeriod, 
												 Par_21_Cost_TravelFix_ByPeriod, Par_21_BigM_S, 
												 Par_21_BigM_E, Par_21_SortimentDemand,
                                                 Fst_21, Per_21, NumBlock_21, Par_21_SolTest);

        public static double Par_OpenNewRoad = 200;
        public static double Par_VolumeVariation = 0.1;
        public static double Par_DemFactor = 0.25;
        public static double Par_LoadingUnloadingCost = 3;
        public static int Par_MaxDistInBlock = 2;
        public static int Par_MaxDistConsecutivePeriod = 2;
        public static int Par_MaxDistOutBlockInPeriod = 3;
        public static int Par_MinimalHarvestAge = 2;
        public static double Par_NewBlockPercent = 0.15;
        public static int Par_NumSortiments = 1;
        public static double Par_TolFactor = 0.5;
        public static int Par_TruckCapacity = 12;
        public static char[] Par_charSeparators = { '\t', ';', ',' };
    }
}
