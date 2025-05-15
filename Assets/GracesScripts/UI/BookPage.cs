using System;
using System.Collections;
using UnityEngine;

namespace Assets.GracesScripts.UI
{
    /// <summary>
    /// Parent class of all the book pages.
    /// </summary>
    public abstract class BookPage : MonoBehaviour
    {
        [SerializeField] GameObject PagesAnimation;
        private Animator animator;

        private void Awake()
        {
            animator = PagesAnimation.GetComponent<Animator>();
            this.PagesAnimation.SetActive(false);

            this.TogglePageComponents(false);
        }

        public void FlipToPage()
        {
            // Flip page animation then enable children of the page.
            StartCoroutine(WaitForPageFlip());
        }

        IEnumerator WaitForPageFlip()
        {
            yield return new WaitForSeconds(0.24f);
            this.PagesAnimation.SetActive(true);
            animator.SetTrigger("Open");
            yield return new WaitForSeconds(0.24f);
            this.PagesAnimation.SetActive(false);

            // enable page contents
            this.TogglePageComponents(true);
        }

        protected abstract void TogglePageComponents(bool OnOff);
    }
}
