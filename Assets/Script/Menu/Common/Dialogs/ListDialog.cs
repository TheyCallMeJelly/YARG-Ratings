using UnityEngine;
using UnityEngine.Events;
using YARG.Helpers.Extensions;
using YARG.Menu.Navigation;
using YARG.Menu.Persistent;
using YARG.Core.Input;

namespace YARG.Menu.Dialogs
{
    public class ListDialog : Dialog
    {
        [Space]
        [SerializeField]
        private Transform _listContainer;
        [SerializeField]
        private ColoredButton _listButtonPrefab;
        [SerializeField]
        private UnityEngine.UI.ScrollRect _scrollRect;

        protected override NavigationScheme GetNavigationScheme()
        {
            return new NavigationScheme(new()
            {
                NavigationScheme.Entry.NavigateSelect,
                NavigationScheme.Entry.NavigateUp,
                NavigationScheme.Entry.NavigateDown,
                new NavigationScheme.Entry(MenuAction.Red, "Menu.Common.Back", () =>
                {
                    DialogManager.Instance.ClearDialog();
                })
            }, null);
        }

        public ColoredButton AddListButton(string text, UnityAction handler, bool closeOnClick = true)
        {
            var button = AddListEntry(_listButtonPrefab);
            button.Text.text = text;

            // Register with navigation group so controller can select it
            var nav = button.GetComponentInChildren<NavigatableUnityButton>();
            if (nav != null)
            {
                _navigationGroup.AddNavigatable(nav);
                nav.SelectionStateChanged += (_, selected, _) =>
                {
                    if (selected && _scrollRect != null)
                    {
                        Canvas.ForceUpdateCanvases();
                        var buttonRect = button.GetComponent<RectTransform>();
                        var scrollable = _scrollRect.ScrollableHeight();
                        if (scrollable > 0)
                        {
                            var viewportHeight = _scrollRect.viewport.rect.height;
                            var buttonY = -buttonRect.anchoredPosition.y;
                            var buttonHeight = buttonRect.rect.height;
                            var currentScroll = (1f - _scrollRect.verticalNormalizedPosition) * scrollable;

                            var buttonTop = buttonY - buttonHeight / 2f;
                            var buttonBottom = buttonY + buttonHeight;

                            if (buttonBottom > currentScroll + viewportHeight)
                                _scrollRect.verticalNormalizedPosition = Mathf.Clamp01(1f - (buttonBottom - viewportHeight) / scrollable);
                            else if (buttonTop < currentScroll)
                                _scrollRect.verticalNormalizedPosition = Mathf.Clamp01(1f - buttonTop / scrollable);
                        }
                    }
                };
            }

            if (closeOnClick)
            {
                button.OnClick.AddListener(() =>
                {
                    handler();
                    DialogManager.Instance.ClearDialog();
                });
            }
            else
            {
                button.OnClick.AddListener(handler);
            }
            return button;
        }

        public T AddListEntry<T>(T prefab)
            where T : Object
        {
            return Instantiate(prefab, _listContainer);
        }

        public void ClearList()
        {
            _listContainer.DestroyChildren();
        }

        public override void ClearDialog()
        {
            base.ClearDialog();

            ClearList();
        }
    }
}
 