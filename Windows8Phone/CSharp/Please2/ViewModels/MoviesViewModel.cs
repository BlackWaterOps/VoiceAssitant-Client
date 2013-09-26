using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Please2.Models;
using Please2.Resources;
using Please2.Util;

namespace Please2.ViewModels
{
    public class MoviesViewModel : NotificationBase
    {
        private string endpoint =  AppResources.Apier + "movies/";

        private Request req;

        private List<MoviesModel> upcoming;

        public List<MoviesModel> Upcoming
        {
            get { return upcoming; }
        }

        private List<MoviesModel> nowPlaying;

        public List<MoviesModel> NowPlaying
        {
            get { return nowPlaying; }
        }

        public async Task GetMovies()
        {
            /*
            req = new Request();

            upcoming = await req.DoRequestJsonAsync<List<MoviesModel>>(endpoint + "upcoming");

            nowPlaying = await req.DoRequestJsonAsync<List<MoviesModel>>(endpoint + "in_theaters");
        
             */
            try
            {
                string tempPayload = "[{\"release_date\": \"2013-08-16\", \"ratings\": {\"critics_score\": 72, \"audience_score\": 83, \"critics_rating\": \"Fresh\", \"audience_rating\": \"Upright\"}, \"mpaa_rating\": \"PG-13\", \"title\": \"Lee Daniels' The Butler\", \"critics_consensus\": \"Gut-wrenching and emotionally affecting,  Lee Daniels' The Butler overcomes an uneven script thanks to strong performances from an all-star cast.\", \"image\": \"http://instart1.flixster.com/movie/11/17/26/11172685_pro.jpg\", \"runtime\": 126, \"year\": 2013, \"id\": \"771271571\", \"abridged_cast\": [{\"name\": \"Forest Whitaker\", \"characters\": [\"Cecil Gaines\"], \"id\": \"162662640\"}, {\"name\": \"Oprah Winfrey\", \"characters\": [\"Gloria Gaines\"], \"id\": \"162654774\"}, {\"name\": \"Cuba Gooding Jr.\", \"characters\": [\"Carter Wilson\"], \"id\": \"162664235\"}, {\"name\": \"Terrence Howard\", \"characters\": [\"Howard\"], \"id\": \"162652991\"}, {\"name\": \"Alan Rickman\", \"characters\": [\"Ronald Reagan\"], \"id\": \"162666132\"}]}]";

                nowPlaying = upcoming = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MoviesModel>>(tempPayload);
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }
    }
}
