using System;
using System.Collections.Generic;
using Assets.Scripts.Models;
using UnityEngine;

#if UNITY_UWP
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
#endif

namespace Assets.Scripts.Azure
{
    public class AzureFunctionService : MonoBehaviour
    {
        public string AzureFnBaseUrl = "https://holobowl.azurewebsites.net/api/";
        public string AzureFnCode = "eL7aVyLqMZRs7WiyBTUTQwRQ2z5A4jUnt3qK5gfciuuxKRS7t4UeRA==";

#if UNITY_UWP
        private HttpClient _httpClient = new HttpClient();

        private void Start()
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Obtiene la lista de puntuaciones
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Score>> GetScores()
        {

            var functionUrl = AzureFnBaseUrl + "GetScores?code=" + AzureFnCode;
            var httpResponse = await _httpClient.GetAsync(new Uri(functionUrl));

            if (!httpResponse.IsSuccessStatusCode) return null;

            var response = await httpResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<Score>>(response);
        }

        /// <summary>
        /// Añade una nueva puntuacion para un usuario, si este usuario ya tuviera otra añadida, se conservará la mayor
        /// </summary>
        /// <param name="score">Puntuacion que se va a añadir</param>
        /// <returns>Devuelve true si se añade correctamente, false si no fue así</returns>
        public async Task<bool> AddScore(Score score)
        {
            var functionUrl = AzureFnBaseUrl + "AddScore?code=" + AzureFnCode;
            var json = JsonConvert.SerializeObject(score);
            var httpResponse = await _httpClient.PostAsync(new Uri(functionUrl), new StringContent(json));

            return httpResponse.IsSuccessStatusCode;
        }
#endif

    }
}
