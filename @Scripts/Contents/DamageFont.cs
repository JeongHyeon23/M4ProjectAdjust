using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Define;

public class DamageFont : MonoBehaviour
{
    private TextMeshPro _damageText;

    public void SetInfo(Vector2 pos, float damage = 0, Transform parent = null)
    {
        _damageText = GetComponent<TextMeshPro>();
        _damageText.sortingOrder = SortingLayers.PROJECTILE;

        transform.position = pos;
        _damageText.color = Color.white;
        if (damage > 0)
            _damageText.text = $"-{Mathf.Abs(damage)}";
        else
        {
            _damageText.color = Color.green;
            _damageText.text = $"+{Mathf.Abs(damage)}";
        }
            
        _damageText.alpha = 1;

        if (parent != null)
            GetComponent<MeshRenderer>().sortingOrder = SortingLayers.DAMAGE_FONT;

        DoAnimation();
    }

    private void DoAnimation()
    {
        Sequence seq = DOTween.Sequence();

        transform.localScale = new Vector3(0, 0, 0);

        seq.Append(transform.DOScale(1.5f, 0.5f).SetEase(Ease.InOutBounce)).
            Join(transform.DOMove(transform.position + Vector3.up, 0.3f).SetEase(Ease.Linear))
            .Append(transform.DOScale(1.5f, 0.5f).SetEase(Ease.InOutBounce))
            .Join(transform.GetComponent<TMP_Text>().DOFade(0, 0.3f).SetEase(Ease.InQuint))
            .OnComplete(() =>
            {
                Managers.Resource.Destroy(gameObject);
            });
    }
}
