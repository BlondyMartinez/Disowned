using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This script is the same as "SadManController" and it is explained there.
public class EnemyController : MonoBehaviour {
	public float hp;
	private TextMesh TM;
	public GameObject TextMeshPrefab;
	public Transform tPos;
	private int expGiven;

	private void Start() {
		TM = TextMeshPrefab.GetComponent<TextMesh>();
		expGiven = 20;
	}

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.CompareTag ("NineBullet")) {
			NineController.DMGController();
			hp -= NineController.NineDMG;

			SetTextNine ();
			CreateTextMesh ();

			if (hp <= 0)  {
				NineController.exp += expGiven;
				EnemyCount.enemyCount--;
				Destroy(gameObject);
			}
			Destroy(other.gameObject);
		}
 
		if (other.gameObject.CompareTag("NineSkill2")) {
			NineController.DMGController();
			hp -= NineController.NineDMG;

			SetTextNine();
			CreateTextMesh();

			if (hp <= 0) {
				NineController.exp += expGiven;
				EnemyCount.enemyCount--;
				Destroy(gameObject);
			}
		}
	}

	void CreateTextMesh () {
		Instantiate (TextMeshPrefab, tPos.position, Quaternion.identity);
	}

	void SetTextNine () {
		TM.fontSize = 20;
		if (NineController.NineDMG == 0) {
			TM.text = "DODGED";
		} else if (NineController.crit) {
			TM.fontSize = 25;
			TM.text = "CRIT " + (int)NineController.NineDMG;
		} else {
			TM.text = "" + (int)NineController.NineDMG;
		}
	}
}
