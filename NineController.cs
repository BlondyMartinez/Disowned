using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NineController : MonoBehaviour {
	private float movementSpeed;
	public static float hp;
	public static float maxHP;
	private Animator anim;
	private bool isAttacking = false;
	public static bool isMoving = false;
	public GameObject bullet;
	public Transform gun1, gun2;
	public static bool crit = false, stunned = false;
	public static float NineDMG;
	public static bool isDead = false;
	public static bool usingSkill2 = false, usingSkill3 = false;
	public GameObject BulletSkill2;
	private float VolteretaCooldown = 0;
	private float Skill2Cooldown = 0;
	private bool skillCoolingDown = false, skill2CoolingDown = false;
	public Slider s2Cooldown, s3Cooldown, sExp;
	public static int exp = 0;
	public static float def = 10;
	public GameObject PartyclesBasicAttack;
	public AudioSource SourceTiro;
	public AudioSource SourceTiroTocho;
	public Image imageHP;
	public Text textHP, textS2, textS3;
	public static int maxDMG, minDMG, lvl;
	public GameObject TextMeshPrefab;
	public Transform posi;
	public static bool deathAnimationDone;
	private bool  lvl2 = false, lvl3 = false, lvl4 = false, lvl5 = false;
	public GameObject lvlUpText;
	public Text lvlText;

	//Used to set static variables value and check game mode chosen by player.
	void Awake() {
		deathAnimationDone = false;

		if (GlobalControl.normalMode) {
			def = 10;
			maxDMG = 15;
			minDMG = 13;
			hp = 500;
			maxHP = 500;
		}

		if (GlobalControl.hardMode) {
			def = 10;
			maxDMG = 15;
			minDMG = 13;
			hp = 200;
			maxHP = 200;
		}

		if (GlobalControl.impossibleMode){
			maxHP = 1;
			hp = 1;
		}

		lvl = 1;
		exp = 0;

		isAttacking = false;
		isMoving = false;
		crit = false;
		stunned = false;
		isDead = false;
		usingSkill2 = false;
		usingSkill3 = false;                          

		textS2.enabled = false;
		textS3.enabled = false;
		
		anim = GetComponent<Animator>();
	}

	//Used fixed update so the amount of times reading it is the every frame, and control all the functions.
	void FixedUpdate() {
		UIController();

		if (!isDead && !PauseMenu.isPaused && !stunned) {
			AnimationController();

			if (!usingSkill2 && !usingSkill3) {
				if (!isAttacking) {
					movementSpeed = 6;
				} else {
					movementSpeed = 2;
				}

				MovementController();
			}

			ExpController();
			LevelSystem();
		}

		if (hp <= 0 && !GlobalControl.practiceMode) {
			hp = 0;
			anim.SetBool("isDead", true);
			isDead = true;
		}
	}

	//Function added to the animation to activate the particle system of one of the skills.
	void EnableBullet() {
		BulletSkill2.SetActive(true);
		SourceTiroTocho.Play();
	}


	//Function added to the animation to disable the particle system of one of the skills.
	void DisableBullet() {
		BulletSkill2.SetActive(false);
	}

	//Function added to the animation to instantiate the partycle systems of the normal attack plus
	//the sound.
	public void SpawnNineBullet1() {
		Instantiate(PartyclesBasicAttack, gun1.position, transform.rotation);
		Instantiate(bullet, gun1.position, transform.rotation);

		SourceTiro.Play();
	}

	//Function added to the animation to instantiate the partycle systems of the normal attack plus
	//the sound.
	public void SpawnNineBullet2() {
		Instantiate(PartyclesBasicAttack, gun2.position, transform.rotation);
		Instantiate(bullet, gun2.position, transform.rotation);

		SourceTiro.Play();
	}

	void MovementController() {
		//When the player is not attacking the movement is always up, down, left and right taking world as reference
		if (!isAttacking) {
			float horizontalMovement = Input.GetAxisRaw("Horizontal");
			float verticalMovement = Input.GetAxisRaw("Vertical");

			Vector3 movement = new Vector3(horizontalMovement, 0.0f, verticalMovement);

			if (movement != Vector3.zero) {
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement.normalized), 0.4f);
			}

			transform.Translate(movement * movementSpeed * Time.deltaTime, Space.World);
		} else {
			//If the player is attacking, the movement takes reference of the character itself and goes forward and backwards rotating if
			//vertical axis is pressed.
			float rotation = Input.GetAxis("VerticalShoot") * 100 * Time.deltaTime;
			float translation = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;

			transform.Translate(0, 0, translation);
			transform.Rotate(0, rotation, 0);
		}
	}

	//Here I control all animations such as second idle, different skills and both shoot standing up and walking
	//plus skill cooldowns.
	void AnimationController() {
		int x = Random.Range(1, 10);
		anim.SetFloat("idle2", x);

		if (Input.GetAxisRaw("Fire1") != 0) {
			anim.SetBool("isAttacking", true);
			isAttacking = true;
		} else {
			anim.SetBool("isAttacking", false);
			isAttacking = false;
		}

		if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) {
			if (!isAttacking) {
				anim.SetBool("isRunning", true);
				isMoving = true;
			}
			anim.SetFloat("atk", 1);
		} else  {
			anim.SetFloat("atk", 0);
			anim.SetBool("isRunning", false);
			isMoving = false;
		}

		if (!usingSkill3) {
			if (Input.GetAxisRaw("Voltereta") != 0 && !skillCoolingDown) {
				anim.Play("Voltereta");
				VolteretaCooldown = 4;
				skillCoolingDown = true;
				usingSkill2 = true;
			}
		}

		if (!usingSkill2) {
			if (Input.GetAxisRaw("NineSkill2") != 0 && !skill2CoolingDown) {
				anim.Play("skill2");
				usingSkill3 = true;
				Skill2Cooldown = 6;
				skill2CoolingDown = true;
			}
		}

		if (skill2CoolingDown) {
			Skill2Cooldown -= Time.deltaTime;

			if (Skill2Cooldown <= 0) {
				skill2CoolingDown = false;
			}
		}

		if (skillCoolingDown) {
			VolteretaCooldown -= Time.deltaTime;
			if (VolteretaCooldown <= 0) {
				skillCoolingDown = false;
			}
		}
	}

	public static void DMGController() {
		//Damage controled by a chance of missing, make a critical hit and normal one randomly.
		//Also added main character's passive skill (damage higher when hp is low)
		int DC = Random.Range(0, 10); {
			//critical damage
			if (DC >= 0 && DC < 1.5f) {
				if (hp <= maxHP * 0.75f && hp > maxHP / 2) {
					NineDMG = Random.Range(minDMG, maxDMG) * 2 * 1.2f;
				} else if (hp <= maxHP / 2 && hp > maxHP / 4) {
					NineDMG = Random.Range(minDMG, maxDMG) * 2 * 1.4f;
				} else if (hp <= maxHP / 4) {
					NineDMG = Random.Range(minDMG, maxDMG) * 2 * 1.6f;
				} else if (hp > maxHP * .75f) {
					NineDMG = Random.Range(minDMG, maxDMG) * 2;
				}

				crit = true;
			} else {
				crit = false;
			}

			//missed
			if (DC >= 1.5f && DC < 2) {
				NineDMG = 0;
			}

			//normal damage
			if (DC >= 2 && DC <= 10) {
				if (hp <= maxHP * 0.75f && hp > maxHP / 2) {
					NineDMG = Random.Range(minDMG, maxDMG) * 1.2f;
				} else if (hp <= maxHP / 2 && hp > maxHP / 4) {
					NineDMG = Random.Range(minDMG, maxDMG) * 1.4f;
				} else if (hp <= maxHP / 4) {
					NineDMG = Random.Range(minDMG, maxDMG) * 1.6f;
				} else if (hp > maxHP * 0.75f) {
					NineDMG = Random.Range(minDMG, maxDMG);
				}
			}
		}
	}

	//Next 3 voids are added to animations to check when the animation is done as the player can't move while them.
	public void SetBoolFalse()
	{
		usingSkill2 = false;
	}

	public void SetBoolFalse2()
	{
		usingSkill3 = false;
	}

	public void SetStunFalse()
	{
		stunned = false;
	}

	//Used a switch statement to increase player stats on each level.
	void LevelSystem() {
		switch (lvl) {
			case 1:
				minDMG = 13;
				maxDMG = 15;
				def = 10;

				sExp.value = exp;
				break;

			case 2:
				if (!lvl2) {
					lvlUpText.SetActive(true);
					lvl2 = true;
				}

				minDMG = 15;
				maxDMG = 17;
				def = 12;

				sExp.maxValue = 50;
				sExp.value = exp - 25;
				break;

			case 3:
				if (!lvl3) {
					lvlUpText.SetActive(true);
					lvl3 = true;
				}

				minDMG = 17;
				maxDMG = 19;
				def = 14;

				sExp.maxValue = 75;
				sExp.value = exp - 75;
				break;

			case 4:
				if (!lvl4) {
					lvlUpText.SetActive(true);
					lvl4 = true;
				}

				minDMG = 19;
				maxDMG = 21;
				def = 16;

				sExp.maxValue = 100;
				sExp.value = exp - 150;
				break;

			case 5:
				if (!lvl5) {
					lvlUpText.SetActive(true);
					lvl5 = true;
				}
				minDMG = 21;
				maxDMG = 23;
				def = 18;
				break;
		}
	}

	//Used to set player's level according to the experience gained.
	void ExpController()
	{
		if (exp >= 0 && exp < 25) {
			lvl = 1;
		} else if (exp >= 25 && exp < 75) {
			lvl = 2;
		} else if (exp >= 75 && exp < 150) {
			lvl = 3;
		} else if (exp >= 150 && exp < 250) {
			lvl = 4;
		} else {
			lvl = 5;
		}
	}
	
	void CreateTextMesh()
	{
		Instantiate(TextMeshPrefab, posi.position, Quaternion.identity);
	}

	public void SlowMoDeath ()
	{
		Time.timeScale = .4f;
	}

	public void SetTimeScaleToNormal ()
	{
		Time.timeScale = 1;
	}

	public void DeathAnimationDone ()
	{
		deathAnimationDone = true;
	}

	//Used to control most of the UI elements.
	void UIController() {
		// Displays level 
		if (lvl != 5) {
			lvlText.text = "LVL " + lvl;
		} else {
			lvlText.text = "MAX LVL";
			sExp.value = sExp.maxValue;
		}

		if (GlobalControl.practiceMode) {
			textHP.text = "∞";
		} else {
			imageHP.fillAmount = (hp / maxHP);
			textHP.text = hp.ToString();
		}

		//Displays cooldown countdown, sets the slider value using cooldown and disables text after cooling down.
		s2Cooldown.value = Skill2Cooldown;
		s3Cooldown.value = VolteretaCooldown;

		if (skill2CoolingDown) {
			textS2.enabled = true;
			textS2.text = "" + (int)Skill2Cooldown;
		} else {
			textS2.enabled = false;
		}

		if (skillCoolingDown) {
			textS3.enabled = true;
			textS3.text = "" + (int)VolteretaCooldown;
		} else {
			textS3.enabled = false;
		}
	}
}