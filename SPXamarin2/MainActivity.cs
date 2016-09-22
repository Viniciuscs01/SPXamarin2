using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using OfficeSolutions.SharePoint;
using Newtonsoft.Json;

namespace SPXamarin2
{
    [Activity(Label = "SPXamarin2", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationAgentContinuationHelper.SetAuthenticationAgentContinuationEventArgs(requestCode, resultCode, data);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);
            
            button.Click += Button_Click;
        }

        private async void Button_Click(object sender, EventArgs e)
        {
            var authResult = await AuthenticationHelper.GetAccessToken(AuthenticationHelper.SharePointURL,
                new PlatformParameters(this));
            //await CreateList(authResult.AccessToken);
            //await CreateItems(authResult.AccessToken);
            await FetchListItems(authResult.AccessToken);
        }

        //protected async Task<bool> CreateList(string token)
        //{
        //    var client = new HttpClient();
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //    var mediaType = new MediaTypeWithQualityHeaderValue("application/A");
        //    mediaType.Parameters.Add(new NameValueHeaderValue("odata", "verbose"));
        //    client.DefaultRequestHeaders.Accept.Add(mediaType);
        //    var body = "{\"__metadata\":{\"type\":\"SP.List\"},\"AllowContentTypes\":true,\"BaseTemplate\":107,\"ContentTypesEnabled\":true,\"Description\":\"Tasks by Xamarin.Android\",\"Title\":\"TasksByAndroid\"}";
        //    var contents = new StringContent(body);
        //    contents.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json;odata=verbose");
        //    try
        //    {
        //        var postResult = await client.PostAsync("https://classsolutions.sharepoint.com/sites/Vinicius/_api/web/lists/", contents);
        //        var result = postResult.EnsureSuccessStatusCode();
        //        Toast.MakeText(this, "List created successfully! Seeding tasks.", ToastLength.Long).Show();
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Toast.MakeText(this, "List already exists! Fetching tasks.", ToastLength.Long).Show();
        //        return false;

        //    }
        //}

        //protected async Task<bool> CreateItems(string token)
        //{
        //    var client = new HttpClient();
        //    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        //    var mediaType = new MediaTypeWithQualityHeaderValue("application/json");
        //    mediaType.Parameters.Add(new NameValueHeaderValue("odata", "verbose"));
        //    client.DefaultRequestHeaders.Accept.Add(mediaType);

        //    var itemToCreateTitle = "Item created on: " + DateTime.Now.ToString("dd/MM HH:mm");
        //    var body = "{\"__metadata\":{\"type\":\"SP.Data.TasksByAndroidListItem\"},\"Title\":\"" + itemToCreateTitle + "\",\"Status\": \"Not Started\"}";
        //    var contents = new StringContent(body);
        //    contents.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json;odata=verbose");
        //    try
        //    {

        //        var postResult = await client.PostAsync("https://classsolutions.sharepoint.com/sites/Vinicius/_api/web/lists/GetByTitle('TasksByAndroid')/items", contents);
        //        var result = postResult.EnsureSuccessStatusCode();
        //        if (result.IsSuccessStatusCode)
        //            Toast.MakeText(this, "List item created successfully!", ToastLength.Long).Show();
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        var msg = "Unable to create list item. " + ex.Message;
        //        Toast.MakeText(this, msg, ToastLength.Long).Show();
        //        return false;
        //    }
        //}

        protected async Task<bool> FetchListItems(string token)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var mediaType = new MediaTypeWithQualityHeaderValue("application/json");
            mediaType.Parameters.Add(new NameValueHeaderValue("odata", "verbose"));
            client.DefaultRequestHeaders.Accept.Add(mediaType);
            try
            {
                var result = await client.GetStringAsync("https://classsolutions.sharepoint.com/sites/Vinicius/_api/web/lists/GetByTitle('Clientes')/items");
                var data = JsonConvert.DeserializeObject<Data>(result);
            }

            catch (Exception ex)
            {
                var msg = "Unable to fetch list items. " + ex.Message;
                Toast.MakeText(this, msg, ToastLength.Long).Show();
            }
            return true;
        }

        public class Data
        {
            public Results d { get; set; }
        }

        public class Results
        {
            public SharePointListItem[] results { get; set; }
        }

        public class SharePointListItem
        {
            public string id { get; set; }
            public string Title { get; set; }
        }
    }
}

