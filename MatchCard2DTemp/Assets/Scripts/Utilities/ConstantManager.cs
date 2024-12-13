using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

//Singleton quản lý hầu hết data của game
public static class ConstantManager
{
	public static UserData userData = new UserData();
	public const string PP_SAVE = "SAVE";
}

public class UserData
{
	public int highScore = 0;
	public int lastScore = 0;
	public UserData()
	{
		//Chỗ này thừa, nếu nhét Load vào khởi tạo, thì khi deserialize string ra sẽ tiếp tục khởi tạo nữa tạo nên loop vô tận
	}

	public void LoadData()
	{
		string dataStr = PlayerPrefs.GetString(ConstantManager.PP_SAVE, "");
		if (dataStr != "")
		{
			//try {
			ConstantManager.userData = JsonConvert.DeserializeObject<UserData>(dataStr);
			//Debug.Log(JsonConvert.DeserializeObject<UserData>(dataStr));
			//}
			//catch(System.Exception e)
			//{
			//    //Debug.LogError(e.Message);
			//    //Debug.LogError(e.StackTrace);
			//    //Debug.LogError(dataStr);
			//}

		}
	}

	public void SaveData()
	{
		PlayerPrefs.SetString(ConstantManager.PP_SAVE, JsonConvert.SerializeObject(ConstantManager.userData));
	}

}