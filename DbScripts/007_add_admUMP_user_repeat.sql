INSERT INTO public.enterprises (id,short_name,full_name,address,email,phone,is_integration,web_site,responsible_person_id,date_on,date_off,inn,kpp,ogrn,account,bank,cor_account,bik) VALUES
	 ('491c4af6-5163-4878-8316-76cc70cf423d','Администрация','Администрация','add1прп','notification-test@logrocon.com','9136326565',true,'www.p.tu',NULL,NULL,NULL,'',NULL,NULL,NULL,NULL,NULL,NULL)
on conflict do nothing
;

insert into persons values(
'fc86612a-07e7-4b02-9c1c-d62dc0b5342e','Администратор UMP','amdUMP','amdUMP','Администратор UMP','491c4af6-5163-4878-8316-76cc70cf423d',null,null,null,null
)
on conflict do nothing
;
insert into users values(
'5ddf8881-b211-42f2-ab82-04c775192d55','admin_UMP','fc86612a-07e7-4b02-9c1c-d62dc0b5342e','AQAAAAIAAYagAAAAEOP9afVW3XDEXJ5zEtVPpgXih+MTRI6Mu/QrWvZfoWCHtowtyZ6W4ZJ2M2y3QyWghQ==',null,null,null,null,false,0
)
on conflict do nothing
;
insert into roles values 
('1d2dfa9b-26f2-48be-aba9-f4a3ee6bf3e9','Администратор UMP',null,'2022-12-13 09:40:46.320923'),
('02f47ee8-3a42-463c-9ccd-3e7d34ca1b74','Слушатель',null,'2023-08-29 15:30:46.320923')
on conflict do nothing
;
insert into user_roles values(
'1d2dfa9b-26f2-48be-aba9-f4a3ee6bf3e9','5ddf8881-b211-42f2-ab82-04c775192d55',null,'2021-12-13 09:40:46.320923'
)
on conflict do nothing
;
insert into role_rights (
select 
'1d2dfa9b-26f2-48be-aba9-f4a3ee6bf3e9',
r.id,
null,
'2022-12-13 09:40:46.320923'
from rights r where date_off is null)
on conflict do nothing
;

insert into persons values(
'fc86612a-07e7-4b02-9c1c-d62dc0b5342e','Администратор UMP','amdUMP','amdUMP','Администратор UMP','491c4af6-5163-4878-8316-76cc70cf423d',null,null,null,null
)
on conflict do nothing
;
insert into users values(
'5ddf8881-b211-42f2-ab82-04c775192d55','admin_UMP','fc86612a-07e7-4b02-9c1c-d62dc0b5342e','AQAAAAIAAYagAAAAEOP9afVW3XDEXJ5zEtVPpgXih+MTRI6Mu/QrWvZfoWCHtowtyZ6W4ZJ2M2y3QyWghQ==',null,null,null,null,false,0
)
on conflict do nothing
;
insert into roles values(
'1d2dfa9b-26f2-48be-aba9-f4a3ee6bf3e9','Администратор UMP',null,'2022-12-13 09:40:46.320923'
)
on conflict do nothing
;
insert into user_roles values(
'1d2dfa9b-26f2-48be-aba9-f4a3ee6bf3e9','5ddf8881-b211-42f2-ab82-04c775192d55',null,'2021-12-13 09:40:46.320923'
)
on conflict do nothing
;
insert into role_rights (
select 
'1d2dfa9b-26f2-48be-aba9-f4a3ee6bf3e9',
r.id,
null,
'2022-12-13 09:40:46.320923'
from rights r where date_off is null)
on conflict do nothing
;
insert into enterprise_data_access_level values(
'ed114bcb-0358-4fae-b1e3-a51b5907e0f9','2020-10-23 12:41:46.743075','491c4af6-5163-4878-8316-76cc70cf423d','e785c2cf-16d5-488f-88ce-b5a81d8d554f', null
)on conflict do nothing
;