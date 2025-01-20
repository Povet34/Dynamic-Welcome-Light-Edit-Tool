using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class App : AbstractSingleton<App>
{
    public SceneMngr Scene;                                 
    public ScreenMngr Screen;                               
    public ResourceMngr Resource;                         
    public LanguageMngr Language;
    public CryptoMngr Crypto;                               
    public UIMngr UI;
    public PopupMngr Popup;
    public DataTableMngr DataTable;

    private List<IMngr> managerLst = new List<IMngr>();

    /// <summary>
    /// UpdateFrame, UpdateSec, Clear�� �ʿ��� �͵鸸 ����Ʈ�� �ִ´�
    /// ���� ���� ��� �ܿ��� �ʿ��ϸ� �߰� �����ϰ� ����ȭ�� ���ؼ��� ���� ��ɵ� �и��ؾ� �� ���� �ִ�
    /// </summary>
    protected virtual void Awake()
    {
        managerLst.Add(Scene);
        managerLst.Add(Screen);
        managerLst.Add(Resource);
        managerLst.Add(Language);
        managerLst.Add(Popup);
        managerLst.Add(UI);
        managerLst.Add(DataTable);

        Init();
        StartCoroutine(UpdateSecBody()); 
    }

    protected virtual void Update()
    {
        IMngr manager;
        for (int index = 0, icount = managerLst.Count; index < icount; ++index)
        {
            manager = managerLst[index];
            if (null != manager)
            {
                manager.UpdateFrame();
            }
        }
    }

    protected virtual void Init()
    {
        IMngr manager;
        for (int index = 0, icount = managerLst.Count; index < icount; ++index)
        {
            manager = managerLst[index];
            if (null != manager)
            {
                manager.Init();
            }
        }
    }

    protected virtual void UpdateSec()
    {
        IMngr manager;
        for (int index = 0, icount = managerLst.Count; index < icount; ++index)
        {
            manager = managerLst[index];
            if (null != manager)
            {
                manager.UpdateSec();
            }
        }
    }

    protected virtual void Clear()
    {
        IMngr manager;
        for (int index = 0, icount = managerLst.Count; index < icount; ++index)
        {
            manager = managerLst[index];
            if (manager != null)
            {
                manager.Clear();
            }
        }
    }

    private IEnumerator UpdateSecBody()
    {
        WaitForSeconds wfs = new WaitForSeconds(1f);
        while (true)
        {
            yield return wfs;
            UpdateSec();
        }
    }
}