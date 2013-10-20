using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Please2.Models;
using Please2.Util;
using Please2.ViewModels;

namespace Please2.Tests
{
    class Single
    {
        private Dictionary<string, object> data = new Dictionary<string, object>();

        private Dictionary<string, object> RealEstateTest()
        {
            try
            {
                var originalQuery = "houese for rent near me";

                var locator = GetLocator();

                var vm = locator.RealEstateViewModel;

                var test = "{\"show\":{\"simple\":{\"text\":\"I've found 5484 listings. The average price is $1543.\"},\"structured\":{\"item\":{\"listings\":[{\"body\":\"WELCOME TO LUXURY IN LAS SENDAS *MAGNIFICENT SINGLE LEVEL GREATROOM FLOORLAN*RARE DIAMOND POINT MODEL*GATED COURTYARD W/FOUNTAIN WELCOMES YOU INTO THIS SPOTLESS,WELL APPOINTED HOME*FULL BATHS IN ALL 3 BEDROOMS+1/2 BATH IN HALL*HUGE 1/3 ACRE PREMIUM CDS LOT*FANTASIC CITY LTS AND MTN VIEWS. SPA, PUTTING GREEN, B/I BBQ, LARGE COVERED PATIO W/C-FANS*B/I SPEAKERS IN PATIO, MASTER AND GREATROOM.CLASSY CLOSETS IN ALL CLOSSETS AND LAUNDRY RM. LAUNDRY RM SINK. B/I GARAGE CABS *OVERSIZED GARAGE* 12+FT CEILINGS T/O .75GAL WTR HTR,SUN SCREENS*WALKIN SNAIL SHOWER*ELECTRIC SUN SHADE ON PATIO*PLANTATION SHUTTERS T/O*42''CABS IN KITCHEN.\",\"category\":{\"id\":\"housing/rent/home\",\"name\":\"Homes for Rent\"},\"revenue_score\":6,\"ctime\":1379463372,\"title\":\"8044 E Sienna St\",\"url\":\"http://apartments.oodle.com/u_a2xx_/3487481235-P28p557,29-14F621F1F930/apar…D0uw2YdD0gFMrPbWk9Rt8zyXAu0tfaSRBkIgq2k6Y_t4TTXMuOBBe2EPWnLVAJXzvwf6E8gF9_\",\"similar_url\":\"http://apartments.oodle.com/85207/homes-for-rent/3-bedrooms/4-bedrooms/price_1500_2600/?r=5\",\"paid\":\"Yes\",\"source\":{\"id\":\"facebook\",\"name\":\"Facebook\"},\"location\":{\"citycode\":\"usa:az:mesa\",\"name\":\"Mesa\",\"zip\":\"85207\",\"country\":\"USA\",\"longitude\":\"-111.6571\",\"state\":\"AZ\",\"address\":\"8044 E Sienna St\",\"latitude\":\"33.4860\",\"addresscode\":\"addr:usa:az:mesa:8044+e+sienna+st\"},\"images\":[{\"src\":\"http://i.oodleimg.com/item/3487481235u_0s_homes_for_rent_in_mesa_az/?1379463372\",\"height\":75,\"width\":100,\"num\":\"0\",\"alt\":\"8044 E Sienna St\",\"size\":\"s\"}],\"attributes\":{\"price_display\":\"$2,300\",\"fee\":\"No\",\"bathrooms\":3.5,\"price\":\"2300\",\"bedrooms\":3,\"square_feet\":2786,\"currency\":\"USD\",\"amenities\":\"Gated,Parking,Patio/Deck\",\"has_photo\":\"Thumbnail\",\"user_id\":\"42847680\"},\"id\":\"3487481235\",\"user\":{\"url\":\"http://www.oodle.com/profile/arizona-real-estate/42847680/\",\"photo\":\"http://graph.facebook.com/150648118301315/picture?type=large\",\"id\":\"42847680\",\"name\":\"Arizona Real Estate\"}},{\"body\":\"MUST SEE!! Classic style home with a midwest / east coast feel and wrap around porch! The home is nestled away from Lone Mountain Road for total privacy. This unique home has an upgraded kitchen and a full second master bedroom on the first floor. Spacious grass backyard perfect for entertaining the kids or guests. Come see this home for yourself and fall in love with its charm, TODAY!!\",\"category\":{\"id\":\"housing/rent/home\",\"name\":\"Homes for Rent\"},\"revenue_score\":2,\"ctime\":1381764805,\"title\":\"House\",\"url\":\"http://apartments.oodle.com/u_a2xx_/3407227674-P1u557,29-14F621F1F930/apart…hFxDD6H-AVn5xsYt3caZYQ2hbP2AEOlDA4JUyyyc7qktiL5l6izahvTvZ0OOz8VwX5RuyHJE1t\",\"similar_url\":\"http://apartments.oodle.com/85331/homes-for-rent/4+-bedrooms/price_1500_2600/?r=5\",\"paid\":\"No\",\"source\":{\"id\":\"www\",\"name\":\"Oodle\"},\"location\":{\"citycode\":\"usa:az:phoenix\",\"name\":\"Phoenix\",\"zip\":\"85331\",\"country\":\"USA\",\"longitude\":\"-111.9632\",\"state\":\"AZ\",\"address\":\"5440 E LONE MOUNTAIN RD\",\"latitude\":\"33.7715\",\"addresscode\":\"addr:usa:az:phoenix:5440+e+lone+mountain\"},\"images\":[{\"src\":\"http://i.oodleimg.com/item/3407227674u_0s_homes_for_rent_in_phoenix_az/?1381764806\",\"height\":75,\"width\":100,\"num\":\"0\",\"alt\":\"House\",\"size\":\"s\"}],\"attributes\":{\"price_display\":\"$2,300\",\"fee\":\"No\",\"bathrooms\":3.5,\"price\":\"2300\",\"bedrooms\":4,\"square_feet\":3084,\"currency\":\"USD\",\"pets_allowed\":\"Dogs\",\"amenities\":\"Patio/Deck\",\"has_photo\":\"Thumbnail\",\"user_id\":\"58065512\"},\"id\":\"3407227674\",\"user\":{\"url\":\"http://www.oodle.com/profile/position-realty/58065512/\",\"photo\":\"http://graph.facebook.com/1028560339/picture?type=large\",\"id\":\"58065512\",\"name\":\"Position Realty\"}},{\"body\":\"Rental Home in Gate community of Lookout Mountainside. 4 beds *2.5baths *Pool *Hot-tub *3-Car garage * Open Floor Plan * Views *Gated. This is a great home on an elevated lot with an ideal floorplan for a family or entertaining. Home has a huge family room with large kitchen, formal dining and living room. Covered patio overlooking the pebble tec pool and beautifully landscaped yard. Much more to see. *Pool and Yard Service include with rent.\",\"category\":{\"id\":\"housing/rent/home\",\"name\":\"Homes for Rent\"},\"revenue_score\":2,\"ctime\":1381601873,\"title\":\"House Rental\",\"url\":\"http://apartments.oodle.com/u_a2xx_/3445090771-P1u557,29-14F621F1F930/apart…pD2eGMLGQ3zkxi8OgapW454axjm79xka9q0n8cK_nqNg1TxoLSJbDKsP7gwVM18-MF8pUyFcZY,\",\"similar_url\":\"http://apartments.oodle.com/85022/homes-for-rent/4+-bedrooms/price_1500_2600/?r=5\",\"paid\":\"No\",\"source\":{\"id\":\"www\",\"name\":\"Oodle\"},\"location\":{\"citycode\":\"usa:az:phoenix\",\"name\":\"Phoenix\",\"zip\":\"85022\",\"country\":\"USA\",\"longitude\":\"-112.0408\",\"state\":\"AZ\",\"address\":\"1928 E Vista Dr.\",\"latitude\":\"33.6222\",\"addresscode\":\"addr:usa:az:phoenix:1928+e+vista+dr\"},\"images\":[{\"src\":\"http://i.oodleimg.com/item/3445090771u_0s_homes_for_rent_in_phoenix_az/?1381601874\",\"height\":75,\"width\":100,\"num\":\"0\",\"alt\":\"House Rental\",\"size\":\"s\"}],\"attributes\":{\"price_display\":\"$2,295\",\"fee\":\"No\",\"bathrooms\":2.5,\"price\":\"2295\",\"bedrooms\":4,\"square_feet\":2801,\"currency\":\"USD\",\"pets_allowed\":\"None\",\"amenities\":\"Gated,Parking,Patio/Deck,Pool\",\"has_photo\":\"Thumbnail\",\"user_id\":\"775297\"},\"id\":\"3445090771\",\"user\":{\"url\":\"http://www.oodle.com/profile/thomas-bartz-phoenix-real-estate/775297/\",\"photo\":\"http://graph.facebook.com/274413855930978/picture?type=large\",\"id\":\"775297\",\"name\":\"Thomas Bartz -Phoenix Real Estate\"}},{\"body\":\"Available November 1st!\n4 Bedroom Home with 2 Baths\nCul de Sac Lot... Great for Kids!\n2 Car Garage\nDesert Landscaping in Frontyard\nGrass in Backyard\",\"category\":{\"id\":\"housing/rent/home\",\"name\":\"Homes for Rent\"},\"revenue_score\":2,\"ctime\":1380729044,\"title\":\"House to Rent 4054 W Windrose Drive, 85029\",\"url\":\"http://apartments.oodle.com/u_a2xx_/3499501572-P1u557,29-14F621F1F930/apart…po4nTpvgl0v5PbpZd9gK2DNo0TuS8R1JAiG4TzbwqYYw1Ce9W4m_S6tI7ZmXR9kmOuV8SG5xnQ,,\",\"similar_url\":\"http://apartments.oodle.com/85029/homes-for-rent/4+-bedrooms/price_800_1400/?r=5\",\"paid\":\"No\",\"source\":{\"id\":\"www\",\"name\":\"Oodle\"},\"location\":{\"citycode\":\"usa:az:phoenix\",\"name\":\"Phoenix\",\"zip\":\"85029\",\"country\":\"USA\",\"longitude\":\"-112.1476\",\"state\":\"AZ\",\"address\":\"4054 W Windrose Drive\",\"latitude\":\"33.6015\",\"addresscode\":\"addr:usa:az:phoenix:4054+w+windrose+driv\"},\"images\":[{\"src\":\"http://i.oodleimg.com/item/3499501572u_0s_homes_for_rent_in_phoenix_az/?1380729044\",\"height\":75,\"width\":100,\"num\":\"0\",\"alt\":\"House to Rent 4054 W Windrose \",\"size\":\"s\"}],\"attributes\":{\"price_display\":\"$1,200\",\"fee\":\"No\",\"bathrooms\":2,\"price\":\"1200\",\"bedrooms\":4,\"square_feet\":1700,\"currency\":\"USD\",\"amenities\":\"Parking\",\"has_photo\":\"Thumbnail\",\"user_id\":\"70658468\"},\"id\":\"3499501572\",\"user\":{\"url\":\"http://www.oodle.com/profile/brad-j/70658468/\",\"photo\":\"http://i.oodleimg.com/a/oodle-profile50x50.gif\",\"id\":\"70658468\",\"name\":\"Brad J.\"}},{\"body\":\"Beautiful 2-story home with 2050 Sqft of living area with swimming pool on a 7200 Sqft lot. In immaculate condition & available for occupancy Sep 17th 2013.\n\n- Matured yard(front & back) and professionally maintaned.(Bi-weekly for Yard). Paid by Owner\n- Pool is maintained weekly by the landlord who gets all the needed chemicals. Paid by Owner\n- Complete bed & bathroom downstairs. \n- Spacious 2 car garage with cabinets. \n- Offers formal dining, formal living, eat-in kitchen, large pantry. \n- Very close to major companies in southeast valley: Intel, Wellsfargo, Orbital, Amkor technology, Honeywell, paypal, ebay\n- Major shopping centers minutes away: Chandler fashion center, phoenix premium outlets \n- Minutes away from major freeways: 101/202/I-10\n- Two beautiful parks in walking distance & has easy access to green belt. \n- Master planned community with elementary & junior high school in walking distance. Schools: Tarwater elementary/Bogle junior high/Hamilton High (on a Short Drive)\n\",\"category\":{\"id\":\"housing/rent/home\",\"name\":\"Homes for Rent\"},\"revenue_score\":2,\"ctime\":1380261555,\"title\":\"5 Br/3Ba home with swimming pool- clemente Ranch\",\"url\":\"http://apartments.oodle.com/u_a2xx_/3495174200-P28u557,29-14F621F1F930/apar…P8HFsJ_BboE2km5o7pqiz490hnW9ve2gFngKV_SdReXD5-LeaxyS0x7jga0J7TAsgSE_zQ6OHB\",\"similar_url\":\"http://apartments.oodle.com/chandler-az/homes-for-rent/4+-bedrooms/price_1000_1800/?r=10\",\"paid\":\"No\",\"source\":{\"id\":\"facebook\",\"name\":\"Facebook\"},\"location\":{\"citycode\":\"usa:az:chandler\",\"name\":\"Chandler\",\"country\":\"USA\",\"longitude\":\"-111.8667\",\"state\":\"AZ\",\"latitude\":\"33.3029\"},\"images\":[{\"src\":\"http://i.oodleimg.com/item/3495174200u_0s_homes_for_rent_in_chandler_az/?1380261555\",\"height\":75,\"width\":100,\"num\":\"0\",\"alt\":\"5 Br/3Ba home with swimming po\",\"size\":\"s\"}],\"attributes\":{\"price_display\":\"$1,595\",\"fee\":\"No\",\"bathrooms\":3,\"price\":\"1595\",\"bedrooms\":5,\"square_feet\":2050,\"currency\":\"USD\",\"pets_allowed\":\"None\",\"amenities\":\"Dishwasher,Parking,Pool,Refrigerator,Washer Dryer\",\"has_photo\":\"Thumbnail\",\"user_id\":\"49059349\"},\"id\":\"3495174200\",\"user\":{\"url\":\"http://www.oodle.com/profile/madhu-m/49059349/\",\"photo\":\"http://graph.facebook.com/100001004085083/picture?type=large\",\"id\":\"49059349\",\"name\":\"Madhu M.\"}},{\"body\":\"$700 / 1br - 250ft² - ONLY ONE ROOM IN A COZY LUXURY HOME IN UPSCALE AREA AVAILABLE FOR RENT (SCOTTSDALE RD & DOUBLETREE PLEASE APPLY ONLY BY PHONE TEXT MSG OR EMAILS WILL BE IGNORED thx\nONLY ONE ROOM IN A COZY LUXURY HOME IN UPSCALE AREA FOR RENT\n$700 / 250ft² - Room available UNBELIEVABLE VIEW Lakefront & Golfcourse (SCOTTSDALE RD AND DOUBLETREE )\nONE MILE VIEW OF MC CORMICK RANCH GOLF COURSE IN YOUR BACKYARD AND LAKEFRONT WITH A PANTOON BOAT ALL KITCHEN SUPPLIES DISHES AND MORE\nNEEDS UPSCALE AND PROFESSIONAL NEEDS THE GOOD CREDIT AND REFRENCES ONE MONTH DEPOSIT AND SHARED THE UTILITY\n1) centrally located (17 MINUTES TO SKY HARBOR AIRPORT) \n2) unique \n3) use of amazing view Located on a private lake. have a pontoon boat E-drive \n4) white cabinets \n5) refrigrator in kitchen ,small microwave ,fireplace,HDtv and small caouch in sitting room, dinningtable in the dinning room \n6) onyx floor in bathroom \n7) Washer& Dryer IN LAUNDR ROOM\n8) Everything you need to enjoy the home: dishes, misc supplies, etc. \",\"category\":{\"id\":\"housing/rent/apartment\",\"name\":\"Apartments for Rent\"},\"revenue_score\":2,\"ctime\":1380125880,\"title\":\"Rentting a Room in My Private Home (Lake and Golfcourse Property\",\"url\":\"http://apartments.oodle.com/u_a2xx_/3455817360-P1u550,29-14F621F1F930/apart…6J3gVk1j-7sDYjMJMmQ2PUZs9tybpFM2YtkNT_9oWPiSMTfJ-VqE3NiRttg7ggAG78JSkT-JjA,,\",\"similar_url\":\"http://apartments.oodle.com/scottsdale-az/apartments-for-rent/3-bedrooms/4-bedrooms/price_460_810/?r=10\",\"paid\":\"No\",\"source\":{\"id\":\"www\",\"name\":\"Oodle\"},\"location\":{\"citycode\":\"usa:az:scottsdale\",\"name\":\"Scottsdale\",\"country\":\"USA\",\"longitude\":\"-111.8902\",\"state\":\"AZ\",\"latitude\":\"33.5783\"},\"images\":[{\"src\":\"http://i.oodleimg.com/item/3455817360u_0s_apartments_for_rent_in_scottsdale_az/?1380125882\",\"height\":75,\"width\":100,\"num\":\"0\",\"alt\":\"Rentting a Room in My Private \",\"size\":\"s\"}],\"attributes\":{\"price_display\":\"$700\",\"fee\":\"No\",\"bathrooms\":2,\"price\":\"700\",\"bedrooms\":3,\"square_feet\":2050,\"currency\":\"USD\",\"pets_allowed\":\"None\",\"amenities\":\"Fireplace,Parking,View,Washer Dryer\",\"has_photo\":\"Thumbnail\",\"user_id\":\"58483163\"},\"id\":\"3455817360\",\"user\":{\"url\":\"http://www.oodle.com/profile/transtar-inc/58483163/\",\"photo\":\"http://graph.facebook.com/100003388433367/picture?type=large\",\"id\":\"58483163\",\"name\":\"TRANSTAR INC\"}},{\"body\":\"Beautiful 2-story home with 2050 Sqft of living area with pool on a 7200 Sqft lot. In immaculate condition & available for immediate occupancy!!\n\n- Brand new carpet\n- Yard maintanance & HOA dues included in the rent and paid by the owner\n- Pool maintance done by the owner weekly. Included in the rent \n- Complete bed & bathroom downstairs. \n- Spacious 2 car garage with cabinets. \n- Offers formal dining, formal living, eat-in kitchen, large pantry. \n- Very close to Intel, Honeywell, paypal, Wellsfargo, Orbital, Amkor technology, Microchip, Chandler Fashion Center, Ocotillo Golf Course. \n- Minutes away from 101/202/I-10 freeways \n- Two beautiful parks in walking distance & has easy access to green belt. \n- Master planned community with elementary & junior high school in walking distance. Schools: Tarwater elementary/Bogle junior high/Hamilton High (on a Short Drive) \n\n- Rent includes everything (except utilities: Gas/Water/Electricity) \n- Freshly painted in & out, tile at all right places. \",\"category\":{\"id\":\"housing/rent/home\",\"name\":\"Homes for Rent\"},\"revenue_score\":2,\"ctime\":1379538282,\"title\":\"5 Bd/3Ba home available for rent immediately\",\"url\":\"http://apartments.oodle.com/u_a2xx_/3488094300-P28u557,29-14F621F1F930/apar…tAoU9LyuTkVkwKdr-Zb0TZqRn-rmDdVll5qSQYtPcqI_pIG3aLBq6vFsYRiLytpIULV6j1_1TA,,\",\"similar_url\":\"http://apartments.oodle.com/chandler-az/homes-for-rent/4+-bedrooms/price_1000_1800/?r=10\",\"paid\":\"No\",\"source\":{\"id\":\"facebook\",\"name\":\"Facebook\"},\"location\":{\"citycode\":\"usa:az:chandler\",\"name\":\"Chandler\",\"country\":\"USA\",\"longitude\":\"-111.8667\",\"state\":\"AZ\",\"latitude\":\"33.3029\"},\"images\":[{\"src\":\"http://i.oodleimg.com/item/3488094300u_0s_homes_for_rent_in_chandler_az/?1379538282\",\"height\":75,\"width\":100,\"num\":\"0\",\"alt\":\"5 Bd/3Ba home available for re\",\"size\":\"s\"}],\"attributes\":{\"price_display\":\"$1,595\",\"fee\":\"No\",\"bathrooms\":3,\"price\":\"1595\",\"bedrooms\":5,\"square_feet\":2050,\"currency\":\"USD\",\"pets_allowed\":\"None\",\"amenities\":\"Dishwasher,Parking,Pool,Refrigerator,Washer Dryer\",\"has_photo\":\"Thumbnail\",\"user_id\":\"49059349\"},\"id\":\"3488094300\",\"user\":{\"url\":\"http://www.oodle.com/profile/madhu-m/49059349/\",\"photo\":\"http://graph.facebook.com/100001004085083/picture?type=large\",\"id\":\"49059349\",\"name\":\"Madhu M.\"}},{\"body\":\"Home in Avondale, Arizona at Avondale Rd. and Van Buren. 4bdr/2.5 baths with a HUGE backyard.\",\"category\":{\"id\":\"housing/rent/home\",\"name\":\"Homes for Rent\"},\"revenue_score\":2,\"ctime\":1379526273,\"title\":\"House\",\"url\":\"http://apartments.oodle.com/u_a2xx_/3488015241-P28u557,29-14F621F1F930/apar…NRiHqHRIDsacrwzEMvqNUnTxHFcdQCcmWXmpJBi09yoj-kgbdosGrq8WxhGIvK2khQtXqPX_VM\",\"similar_url\":\"http://apartments.oodle.com/avondale-az/homes-for-rent/4+-bedrooms/price_700_1300/?r=10\",\"paid\":\"No\",\"source\":{\"id\":\"facebook\",\"name\":\"Facebook\"},\"location\":{\"citycode\":\"usa:az:avondale\",\"name\":\"Avondale\",\"country\":\"USA\",\"longitude\":\"-112.3453\",\"state\":\"AZ\",\"latitude\":\"33.4305\"},\"images\":[{\"src\":\"http://i.oodleimg.com/item/3488015241u_0s_homes_for_rent_in_avondale_az/?1379526274\",\"height\":75,\"width\":100,\"num\":\"0\",\"alt\":\"House\",\"size\":\"s\"}],\"attributes\":{\"price_display\":\"$1,100\",\"fee\":\"No\",\"bathrooms\":2.5,\"price\":\"1100\",\"bedrooms\":4,\"square_feet\":1900,\"currency\":\"USD\",\"pets_allowed\":\"Cats,Dogs\",\"has_photo\":\"Thumbnail\",\"user_id\":\"70621709\"},\"id\":\"3488015241\",\"user\":{\"url\":\"http://www.oodle.com/profile/stacey-h/70621709/\",\"photo\":\"http://graph.facebook.com/1504432473/picture?type=large\",\"id\":\"70621709\",\"name\":\"Stacey H.\"}},{\"body\":\"Rare opportunity to rent this great home*you will be in walkable distance to school, near shopping, restaurants, and freeways, but still tucked away in the lovely mountains of west wing*this home has 4 beds and two bathrooms, formal living and dining, as well as a family room*kitchen has stainless appliances and you have new carpet, fresh paint, and tile in all the right places*jack and jill bath, c-fans, low-maintenance and private back yard, protected patio*this home is very clean and ready for you*\",\"category\":{\"id\":\"housing/rent/home\",\"name\":\"Homes for Rent\"},\"revenue_score\":0,\"ctime\":1381959878,\"title\":\"Four BR in Westwing Mountain Homes Houses Rental For Rent in Peoria Arizona\",\"url\":\"http://apartments.oodle.com/j_a2xx_/3510598519-21257u557,29-14F621F1F930/ap…QcOcCMLPVAQ2hbP2AEOlDA4JUyyyc7qIYANwUvNrbsTQg3DxTyVeTHuOBrQntMCyBIT_NDo4cE,\",\"similar_url\":\"http://apartments.oodle.com/85383/homes-for-rent/4+-bedrooms/price_900_1600/?r=5\",\"paid\":\"No\",\"source\":{\"id\":\"propertynut-realestate\",\"name\":\"PropertyNut.Com\"},\"location\":{\"citycode\":\"usa:az:peoria\",\"name\":\"Peoria\",\"zip\":\"85383\",\"country\":\"USA\",\"longitude\":\"-112.2429\",\"state\":\"AZ\",\"address\":\"26933 84th Ave\",\"latitude\":\"33.7268\",\"addresscode\":\"addr:usa:az:peoria:26933+84th+avenue\"},\"images\":[{\"src\":\"http://i.oodleimg.com/item/3510598519t_1s_homes_for_rent_in_peoria_az/?1381960038\",\"height\":75,\"width\":100,\"num\":\"1\",\"alt\":\"Four BR in Westwing Mountain H\",\"size\":\"s\"}],\"attributes\":{\"price_display\":\"$1,395\",\"fee\":\"No\",\"bathrooms\":2,\"price\":\"1395\",\"bedrooms\":4,\"square_feet\":2001,\"currency\":\"USD\",\"amenities\":\"Patio/Deck\",\"has_photo\":\"Thumbnail\"},\"id\":\"3510598519\",\"user\":[]},{\"body\":\"Spacious yet surprising cozy! The interior of this single level phoenix home welcomes you with a spacious living room, bay windows adorning the dedicated dining area room, and a must see kitchen that comes with gorgeous stainless appliances!\",\"category\":{\"id\":\"housing/rent/home\",\"name\":\"Homes for Rent\"},\"revenue_score\":2,\"ctime\":1381916551,\"title\":\"8664 W Holly St\",\"url\":\"http://apartments.oodle.com/j_a2xx_/3510287749-11403u557,29-14F621F1F930/ap…fZNJIf3zN-feBCvMFmpLV928MU4n4WnH9I3Xl0zdW4-YlpZuS3KCqkm3KfajWYpb90hzsDTsRA,\",\"similar_url\":\"http://apartments.oodle.com/85037/homes-for-rent/3-bedrooms/4-bedrooms/price_620_1060/?r=5\",\"paid\":\"No\",\"source\":{\"id\":\"realrentals\",\"name\":\"RealRentals\"},\"location\":{\"citycode\":\"usa:az:phoenix\",\"name\":\"Phoenix\",\"zip\":\"85037\",\"country\":\"USA\",\"longitude\":\"-112.2469\",\"state\":\"AZ\",\"address\":\"8664 W Holly St\",\"latitude\":\"33.4699\",\"addresscode\":\"addr:usa:az:phoenix:8664+w+holly+st\"},\"images\":[{\"src\":\"http://i.oodleimg.com/item/3510287749t_1s_homes_for_rent_in_phoenix_az/?1381916791\",\"height\":75,\"width\":100,\"num\":\"1\",\"alt\":\"8664 W Holly St\",\"size\":\"s\"}],\"attributes\":{\"price_display\":\"$950 Yearly\",\"fee\":\"No\",\"bathrooms\":2,\"price_type\":\"Yearly\",\"price\":\"950\",\"bedrooms\":3,\"currency\":\"USD\",\"pets_allowed\":\"Cats,Dogs\",\"amenities\":\"AC,Fireplace,Parking,Pool\",\"has_photo\":\"Thumbnail\"},\"id\":\"3510287749\",\"user\":[]}],\"stats\":{\"price\":{\"std\":558.132,\"min\":700,\"max\":2300,\"median\":1595,\"range\":1600,\"mode\":1595,\"mean\":1543},\"bedrooms\":{\"std\":1,\"min\":3,\"max\":5,\"median\":4,\"range\":2,\"mode\":4,\"mean\":3},\"bathrooms\":{\"std\":0,\"min\":2,\"max\":3,\"median\":2,\"range\":1,\"mode\":2,\"mean\":2},\"square_feet\":{\"std\":458.277,\"min\":1700,\"max\":3084,\"median\":2050,\"range\":1384,\"mode\":2050,\"mean\":2269}}},\"template\":\"single:real_estate\"}},\"speak\":\"I've found 5484 listings. The average price is $1543.\"}";

                var actor = JsonConvert.DeserializeObject<ActorModel>(test);

                var show = actor.show;

                var realestateResults = (show.structured["item"] as JObject).ToObject<RealEstateModel>();

                vm.Listings = realestateResults.listings;
                vm.Stats = realestateResults.stats;

                data.Add("title", "real estate");
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
            return data;
        }

        private Dictionary<string, object> GeopoliticsTest()
        {
            var originalQuery = "population of south africa";

            var locator = GetLocator();

            var vm = locator.GeopoliticsViewModel;

            var test = "{\"show\":{\"simple\":{\"text\":\"The population of South Africa is 48,601,098.\"},\"structured\":{\"item\":{\"country\":\"South Africa\",\"flag\":\"http://stremor-apier.appspot.com/static/flags/sf-lgflag.gif\",\"stats\":{\"area\":\"1,219,090\",\"leader\":\"President Jacob Zuma\",\"population\":\"48,601,098\"}},\"template\":\"single:geopolitics\"}},\"speak\":\"The population of South Africa is 48,601,098.\"}";
            
            var actor = JsonConvert.DeserializeObject<ActorModel>(test);

            var show = actor.show;

            var geoResults = (show.structured["item"] as JObject).ToObject<GeopoliticsModel>();

            vm.Flag = geoResults.flag;
            vm.Country = geoResults.country;
            vm.Stats = geoResults.stats;
             
            data.Add("title", "fact book");

            return data;
        }

        private Dictionary<string, object> FlightsTest()
        {
            var originalQuery = "status of united flight 1053";

            var locator = GetLocator();

            var vm = locator.FlightsViewModel;

            var test = "{\"show\":{\"simple\":{\"text\":\"UAL1053 departed at 10:29 pm yesterday, and is set to arrive in Cleveland, OH at 12:33 am.\"},\"structured\":{\"item\":{\"flight_number\":\"1053\",\"airline\":{\"code\":\"UAL\",\"name\":\"United Air Lines Inc.\",\"url\":\"http://www.united.com/\",\"country\":\"US\",\"phone\":\"+1-800-225-5833\",\"callsign\":\"United\",\"location\":\"\",\"shortname\":\"United\"},\"details\":[{\"origin\":{\"city\":\"Newark, NJ\",\"airport_code\":\"KEWR\",\"airport_name\":\"Newark Liberty Intl\"},\"status\":\"departed\",\"schedule\":{\"estimated_arrival\":\"2013-10-12T00:33:34\",\"actual_departure\":\"2013-10-11T23:38:00\",\"filed_departure\":\"2013-10-11T22:29:00\"},\"destination\":{\"city\":\"Cleveland, OH\",\"airport_code\":\"KCLE\",\"airport_name\":\"Cleveland-Hopkins Intl\"},\"delay\":44,\"identification\":\"UAL1053\"}]},\"template\":\"single:flights\"}},\"speak\":\"UAL1053 departed at 10:29 pm yesterday, and is set to arrive in Cleveland, OH at 12:33 am.\"}";

            var actor = JsonConvert.DeserializeObject<ActorModel>(test);

            var show = actor.show;

            var flightResults = (show.structured["item"] as JObject).ToObject<FlightModel>();

            vm.Flights = flightResults.details;
            vm.Airline = flightResults.airline;
            vm.FlightNumber = flightResults.flight_number;

            data.Add("title", "flights");
            data.Add("subtitle", String.Format("flight results for \"{0}\"", originalQuery));

            return data;
        }

        private Dictionary<string, object> NewsTest()
        {
            var originalQuery = "what's up with obama";

            var locator = GetLocator();

            var vm = locator.NewsViewModel;

            var test = "{\"show\":{\"simple\":{\"text\":\"According to news.yahoo.com, President Barack Obama has chosen Janet Yellen to lead the Federal Reserve in a move expected to sustain outgoing chairman Ben Bernanke's focus on cutting joblessness in the still-struggling economy. Obama was to announce his nomination of Yellen, currently Fed vice chair, at a White House event at 1900 GMT Wed. also attended by Bernanke, an official said.\",\"link\":\"http://news.yahoo.com/obama-nominate-yellen-federal-chief-231027971.html\"},\"structured\":{\"items\":[{\"description\":\"Washington (AFP) - President Barack Obama has chosen Janet Yellen to lead the Federal Reserve in a move expected to sustain outgoing chairman Ben Bernanke's focus on cutting joblessness in the still-struggling economy. The move would put a woman at the ...\",\"title\":\"Obama chooses Yellen to lead Fed\",\"url\":\"http://news.yahoo.com/obama-nominate-yellen-federal-chief-231027971.html\",\"summary\":\"President Barack Obama has chosen Janet Yellen to lead the Federal Reserve in a move expected to sustain outgoing chairman Ben Bernanke's focus on cutting joblessness in the still-struggling economy. Obama was to announce his nomination of Yellen, currently Fed vice chair, at a White House event at 1900 GMT Wed. also attended by Bernanke, an official said.\",\"source\":\"Yahoo! News\",\"date\":\"2013-10-09T15:41:20Z\"},{\"url\":\"http://www.bloomberg.com/news/2013-10-09/obama-seeks-post-debt-talks-with-senate-republicans-open.html\",\"source\":\"Bloomberg\",\"date\":\"2013-10-09T14:36:54Z\",\"description\":\"Giving priority to interest payments would prevent the U.S. from defaulting on its debt while requiring the government to balance its budget ... to go home,” said Stu Shea, president and chief operating officer of Leidos. A debt-ceiling ...\",\"title\":\"Obama Seeks Post-Debt Talks With Senate Republicans Open\"},{\"description\":\"WASHINGTON — President Obama will meet with congressional caucuses from both parties in the coming days, starting Wednesday in a session with House Democrats. The meeting is scheduled for 4:30 p.m. ET, and Obama and the Democrats will discuss ...\",\"title\":\"Obama to meet with congressional caucuses\",\"url\":\"http://www.usatoday.com/story/news/politics/2013/10/09/obama-house-senate-government-shutdown/2950557/\",\"summary\":\"WASHINGTON President Obama will meet with congressional caucuses from both parties in the coming days, starting Wed. in a session with House Democrats. The meeting is scheduled for 4:30 p.m. ET, and Obama and the Democrats will discuss the ongoing government shutdown and the prospect of a debt ceiling default. In recent days, Obama has refused Republican requests to negotiate a new spending plan that would end the government shutdown now in its ninth day.\",\"source\":\"USA Today\",\"date\":\"2013-10-09T14:22:35Z\"}],\"template\":\"list:news\"}},\"speak\":\"According to news.yahoo.com, President Barack Obama has chosen Janet Yellen to lead the Federal Reserve in a move expected to sustain outgoing chairman Ben Bernanke's focus on cutting joblessness in the still-struggling economy. Obama was to announce his nomination of Yellen, currently Fed vice chair, at a White House event at 1900 GMT Wed. also attended by Bernanke, an official said.\"}";

            var actor = Newtonsoft.Json.JsonConvert.DeserializeObject<ActorModel>(test);

            var show = actor.show;

            IEnumerable<object> stories = ((JToken)show.structured["items"]).ToObject<IEnumerable<NewsModel>>();

            vm.Stories = stories.Cast<NewsModel>().ToList<NewsModel>();

            data.Add("title", "news results");
            data.Add("subtitle", String.Format("news search on \"{0}\"", originalQuery));

            return data;
        }

        private Dictionary<string, object> StockTest()
        {
            var locator = GetLocator();

            var vm = locator.StockViewModel;

            var test = "{\"show\":{\"simple\":{\"text\":\"Facebook stock is trading at $49.19, down 2.17%\"},\"structured\":{\"item\":{\"opening_price\":50.46,\"low_price\":49.06,\"P/E Ratio\":227.51,\"share_price\":49.19,\"stock_exchange\":\"NasdaqNM\",\"trade_volume\":\"48.20 million\",\"52_week_high\":51.6,\"average_trade_volume\":\"70.12 million\",\"pe\":0.221,\"high_price\":50.72,\"share_price_change\":-1.09,\"market_cap\":\"119.80 billion\",\"5_day_moving_average\":43.542,\"symbol\":\"FB\",\"share_price_change_percent\":2.17,\"name\":\"Facebook\",\"yield\":\"N/A\",\"52_week_low\":18.8,\"share_price_direction\":\"down\"},\"template\":\"simple:stock\"}},\"speak\":\"Facebook stock is trading at $49.19, down 2.17%\"}";

            var actor = JsonConvert.DeserializeObject<Please2.Models.ActorModel>(test);

            var show = actor.show;

            vm.StockData = ((JToken)show.structured["item"]).ToObject<StockModel>();

            var direction = vm.StockData.share_price_direction;

            if (direction == "down")
            {
                vm.DirectionSymbol = "\uf063";
                vm.DirectionColor = "#dc143c";
            }
            else if (direction == "up")
            {
                vm.DirectionSymbol = "\uf062";
                vm.DirectionColor = "#008000";
            }

            data.Add("title", "stock results");

            return data;
        }

        private Dictionary<string, object> WeatherTest()
        {
            var locator = GetLocator();

            var vm = locator.WeatherViewModel;

            var test = "{\"show\":{\"simple\":{\"text\":\"Here's our forecast for Wednesday: Mostly clear, with a low around 65. Southwest wind around 5 mph becoming calm  in the evening.\"},\"structured\":{\"item\":{\"week\":[{\"date\":\"2013-10-02\",\"night\":{\"text\":\"Mostly clear, with a low around 65. Southwest wind around 5 mph becoming calm  in the evening. \",\"sky\":\"Mostly Clear\",\"temp\":\"65\"}},{\"date\":\"2013-10-03\",\"daytime\":{\"text\":\"Sunny, with a high near 89. Light and variable wind becoming southwest 5 to 10 mph in the morning. \",\"sky\":\"Sunny\",\"temp\":\"89\"},\"night\":{\"text\":\"Mostly clear, with a low around 63. West wind 5 to 9 mph becoming light and variable. \",\"sky\":\"Mostly Clear\",\"temp\":\"63\"}},{\"date\":\"2013-10-04\",\"daytime\":{\"text\":\"Sunny, with a high near 86. Light and variable wind becoming northwest around 6 mph in the afternoon. \",\"sky\":\"Sunny\",\"temp\":\"86\"},\"night\":{\"text\":\"Mostly clear, with a low around 59. Breezy, with a north northwest wind 9 to 14 mph becoming northeast 15 to 20 mph after midnight. Winds could gust as high as 28 mph. \",\"sky\":\"Breezy\",\"temp\":\"59\"}},{\"date\":\"2013-10-05\",\"daytime\":{\"text\":\"Sunny, with a high near 85. Breezy. \",\"sky\":\"Breezy\",\"temp\":\"85\"},\"night\":{\"text\":\"Mostly clear, with a low around 57.\",\"sky\":\"Mostly Clear\",\"temp\":\"57\"}},{\"date\":\"2013-10-06\",\"daytime\":{\"text\":\"Sunny, with a high near 88.\",\"sky\":\"Sunny\",\"temp\":\"88\"},\"night\":{\"text\":\"Mostly clear, with a low around 61.\",\"sky\":\"Mostly Clear\",\"temp\":\"61\"}},{\"date\":\"2013-10-07\",\"daytime\":{\"text\":\"Sunny, with a high near 90.\",\"sky\":\"Sunny\",\"temp\":\"90\"},\"night\":{\"text\":\"Mostly clear, with a low around 62.\",\"sky\":\"Mostly Clear\",\"temp\":\"62\"}},{\"date\":\"2013-10-08\",\"daytime\":{\"text\":\"Sunny, with a high near 88.\",\"sky\":\"Sunny\",\"temp\":\"88\"},\"night\":{\"text\":\"Mostly clear, with a low around 63.\",\"sky\":\"Mostly Clear\",\"temp\":\"63\"}}],\"now\":{\"sky\":\"Mostly Clear\",\"temp\":\"89\"},\"location\":null},\"template\":\"single:weather\"}},\"speak\":\"Here's our forecast for Wednesday: Mostly clear, with a low around 65. Southwest wind around 5 mph becoming calm  in the evening.\"}";

            var actor = Newtonsoft.Json.JsonConvert.DeserializeObject<ActorModel>(test);

            var show = actor.show;

            var weatherResults = ((JToken)show.structured["item"]).ToObject<WeatherModel>();

            // since the api drops the daytime info part way through the afternoon, 
            // lets fill in the missing pieces with what we do have
            var today = weatherResults.week[0];

            if (today.daytime == null)
            {
                today.daytime = new WeatherDayDetails()
                {
                    temp = weatherResults.now.temp,
                    text = today.night.text
                };
            }

            vm.MultiForecast = weatherResults.week;

            vm.CurrentCondition = weatherResults.now;

            data.Add("title", "weather results");

            return data;
        }

        private Dictionary<string, object> FitbitTest()
        {
            var locator = GetLocator();

            var vm = locator.FitbitViewModel;

            var test = "{\"show\":{\"simple\":{\"text\":\"As of October 7 2013 your weight is 250.9 pounds\"},\"structured\":{\"item\":{\"goals\":{\"weight\":210},\"timeseries\":[{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-08\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-09\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-10\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-11\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-12\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-13\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-14\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-15\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-16\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-17\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-18\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-19\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-20\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-21\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-22\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-23\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-24\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-25\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-26\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-27\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-28\"},{\"value\":\"250.86000061035156\",\"dateTime\":\"2013-09-29\"},{\"value\":\"250.89999389648438\",\"dateTime\":\"2013-09-30\"},{\"value\":\"250.89999389648438\",\"dateTime\":\"2013-10-01\"},{\"value\":\"250.89999389648438\",\"dateTime\":\"2013-10-02\"},{\"value\":\"250.89999389648438\",\"dateTime\":\"2013-10-03\"},{\"value\":\"250.89999389648438\",\"dateTime\":\"2013-10-04\"},{\"value\":\"250.89999389648438\",\"dateTime\":\"2013-10-05\"},{\"value\":\"250.89999389648438\",\"dateTime\":\"2013-10-06\"},{\"value\":\"250.89999389648438\",\"dateTime\":\"2013-10-07\"}]},\"template\":\"single:fitbit:weight\"}},\"speak\":\"As of October 7 2013 your weight is 250.9 pounds\"}";

            var actor = Newtonsoft.Json.JsonConvert.DeserializeObject<ActorModel>(test);

            var show = actor.show;

            var fitbitResults = ((JToken)show.structured["item"]).ToObject<FitbitWeightModel>();

            vm.Points = fitbitResults.timeseries;
            vm.Goals = fitbitResults.goals;

            data.Add("title", "fitbit results");

            return data;
        }

        private Dictionary<string, object> FitbitFoodTest()
        {
            var locator = GetLocator();

            var vm = locator.FitbitViewModel;

            var test = "{\"show\":{\"simple\":{\"text\":\"I'm sorry. I cannot update your food log right now\"},\"structured\":{\"item\":{\"foods\":[{\"logId\":397779132,\"loggedFood\":{\"mealTypeId\":7,\"foodId\":4843,\"locale\":\"en_US\",\"brand\":\"\",\"calories\":90,\"amount\":100,\"units\":[226,180,147,389],\"accessLevel\":\"PUBLIC\",\"unit\":{\"plural\":\"grams\",\"id\":147,\"name\":\"gram\"},\"name\":\"Banana\"},\"nutritionalValues\":{\"carbs\":20,\"fiber\":2,\"sodium\":2,\"calories\":90,\"fat\":0.5,\"protein\":1},\"logDate\":\"2013-10-10\",\"isFavorite\":false},{\"logId\":397781203,\"loggedFood\":{\"mealTypeId\":7,\"foodId\":82393,\"locale\":\"en_US\",\"brand\":\"\",\"calories\":100,\"amount\":8,\"units\":[304,179,204,319,209,189,128,364,349,91,256,279,401,226,180,147,389],\"accessLevel\":\"PUBLIC\",\"unit\":{\"plural\":\"fl oz\",\"id\":128,\"name\":\"fl oz\"},\"name\":\"Pepsi\"},\"nutritionalValues\":{\"carbs\":27.5,\"fiber\":0,\"sodium\":25.5,\"calories\":100,\"fat\":0,\"protein\":0},\"logDate\":\"2013-10-10\",\"isFavorite\":false},{\"logId\":397782407,\"loggedFood\":{\"mealTypeId\":7,\"foodId\":42111,\"locale\":\"en_US\",\"brand\":\"Johnny Rockets\",\"calories\":170,\"amount\":1,\"units\":[304],\"accessLevel\":\"PUBLIC\",\"unit\":{\"plural\":\"servings\",\"id\":304,\"name\":\"serving\"},\"name\":\"Coke\"},\"nutritionalValues\":{\"carbs\":28,\"fiber\":0,\"sodium\":80,\"calories\":170,\"fat\":0,\"protein\":0},\"logDate\":\"2013-10-10\",\"isFavorite\":false},{\"logId\":397787791,\"loggedFood\":{\"mealTypeId\":7,\"foodId\":81313,\"locale\":\"en_US\",\"brand\":\"\",\"calories\":193,\"amount\":1,\"units\":[304,226,180,147,389],\"accessLevel\":\"PUBLIC\",\"unit\":{\"plural\":\"servings\",\"id\":304,\"name\":\"serving\"},\"name\":\"Cheese\"},\"nutritionalValues\":{\"carbs\":32.5,\"fiber\":3,\"sodium\":490,\"calories\":193,\"fat\":8.5,\"protein\":11.5},\"logDate\":\"2013-10-10\",\"isFavorite\":false},{\"logId\":397796670,\"loggedFood\":{\"mealTypeId\":7,\"foodId\":4843,\"locale\":\"en_US\",\"brand\":\"\",\"calories\":77,\"amount\":3,\"units\":[226,180,147,389],\"accessLevel\":\"PUBLIC\",\"unit\":{\"plural\":\"oz\",\"id\":226,\"name\":\"oz\"},\"name\":\"Banana\"},\"nutritionalValues\":{\"carbs\":17,\"fiber\":1.5,\"sodium\":1.5,\"calories\":77,\"fat\":0.5,\"protein\":1},\"logDate\":\"2013-10-10\",\"isFavorite\":false},{\"logId\":397799001,\"loggedFood\":{\"mealTypeId\":7,\"foodId\":4843,\"locale\":\"en_US\",\"brand\":\"\",\"calories\":90,\"amount\":100,\"units\":[226,180,147,389],\"accessLevel\":\"PUBLIC\",\"unit\":{\"plural\":\"grams\",\"id\":147,\"name\":\"gram\"},\"name\":\"Banana\"},\"nutritionalValues\":{\"carbs\":20,\"fiber\":2,\"sodium\":2,\"calories\":90,\"fat\":0.5,\"protein\":1},\"logDate\":\"2013-10-10\",\"isFavorite\":false},{\"logId\":397899468,\"loggedFood\":{\"mealTypeId\":7,\"foodId\":4843,\"locale\":\"en_US\",\"brand\":\"\",\"calories\":90,\"amount\":100,\"units\":[226,180,147,389],\"accessLevel\":\"PUBLIC\",\"unit\":{\"plural\":\"grams\",\"id\":147,\"name\":\"gram\"},\"name\":\"Banana\"},\"nutritionalValues\":{\"carbs\":20,\"fiber\":2,\"sodium\":2,\"calories\":90,\"fat\":0.5,\"protein\":1},\"logDate\":\"2013-10-10\",\"isFavorite\":false},{\"logId\":397903970,\"loggedFood\":{\"mealTypeId\":7,\"foodId\":4843,\"locale\":\"en_US\",\"brand\":\"\",\"calories\":90,\"amount\":100,\"units\":[226,180,147,389],\"accessLevel\":\"PUBLIC\",\"unit\":{\"plural\":\"grams\",\"id\":147,\"name\":\"gram\"},\"name\":\"Banana\"},\"nutritionalValues\":{\"carbs\":20,\"fiber\":2,\"sodium\":2,\"calories\":90,\"fat\":0.5,\"protein\":1},\"logDate\":\"2013-10-10\",\"isFavorite\":false},{\"logId\":397911826,\"loggedFood\":{\"mealTypeId\":7,\"foodId\":4843,\"locale\":\"en_US\",\"brand\":\"\",\"calories\":90,\"amount\":100,\"units\":[226,180,147,389],\"accessLevel\":\"PUBLIC\",\"unit\":{\"plural\":\"grams\",\"id\":147,\"name\":\"gram\"},\"name\":\"Banana\"},\"nutritionalValues\":{\"carbs\":20,\"fiber\":2,\"sodium\":2,\"calories\":90,\"fat\":0.5,\"protein\":1},\"logDate\":\"2013-10-10\",\"isFavorite\":false},{\"logId\":397913846,\"loggedFood\":{\"mealTypeId\":7,\"foodId\":4843,\"locale\":\"en_US\",\"brand\":\"\",\"calories\":90,\"amount\":100,\"units\":[226,180,147,389],\"accessLevel\":\"PUBLIC\",\"unit\":{\"plural\":\"grams\",\"id\":147,\"name\":\"gram\"},\"name\":\"Banana\"},\"nutritionalValues\":{\"carbs\":20,\"fiber\":2,\"sodium\":2,\"calories\":90,\"fat\":0.5,\"protein\":1},\"logDate\":\"2013-10-10\",\"isFavorite\":false}],\"goals\":{\"calories\":1669,\"estimatedCaloriesOut\":2169},\"summary\":{\"carbs\":226.54649353027344,\"fiber\":15.330870628356934,\"sodium\":609.0343017578125,\"calories\":1080,\"fat\":10.555147171020508,\"water\":0,\"protein\":19.55388069152832}},\"template\":\"single:fitbit:log-food\"}},\"speak\":\"I'm sorry. I cannot update your food log right now\"}";

            var actor = Newtonsoft.Json.JsonConvert.DeserializeObject<ActorModel>(test);

            var show = actor.show;

            var fitbitResults = ((JToken)show.structured["item"]).ToObject<FitbitFoodModel>();

            //Debug.WriteLine(JsonConvert.SerializeObject(fitbitResults));

            var remaining = fitbitResults.goals.calories - fitbitResults.summary.calories;

            vm.Foods = fitbitResults.foods;
            vm.FoodGoals = fitbitResults.goals;
            vm.FoodSummary = fitbitResults.summary;
            vm.CaloriesRemaining = (remaining > 0) ? remaining : 0;

            data.Add("title", "fitbit food results");
            data.Add("titlevisibility", Visibility.Collapsed);

            return data;
        }

        private Dictionary<string, object> HoroscopeTest()
        {
            var locator = GetLocator();

            var vm = locator.HoroscopeViewModel;

            var test = "{\"show\":{\"simple\":{\"text\":\"Today you will be unlucky in family. Keep an eye out for a man in blue to have a major impact on your week. Your lucky number for today is 1.\"},\"structured\":{\"item\":{\"zodiac_sign\":\"leo\",\"horoscope\":\"Today you will be unlucky in family. Keep an eye out for a man in blue to have a major impact on your week. Your lucky number for today is 1.\"},\"template\":\"single:horoscope\"}},\"speak\":\"Today you will be unlucky in family. Keep an eye out for a man in blue to have a major impact on your week. Your lucky number for today is 1.\"}";

            var actor = Newtonsoft.Json.JsonConvert.DeserializeObject<ActorModel>(test);

            var show = actor.show;

            var horoscopeResults = ((JToken)show.structured["item"]).ToObject<HoroscopeModel>();

            var sign = horoscopeResults.zodiac_sign;

            vm.ZodiacSign = sign;
            vm.Horoscope = horoscopeResults.horoscope;

            var date = DateTime.Now.ToString("dddd, MMMM d, yyyy");

            data.Add("title", "horoscope");
            data.Add("subtitle", sign + " for " + date);

            return data;
        }

        private ViewModelLocator GetLocator()
        {
            return App.Current.Resources["Locator"] as ViewModelLocator;
        }
    }
}
